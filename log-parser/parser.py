# -*- coding: utf-8 -*-

# Settings for irssi
#
# log_timestamp = "%Y-%m-%dT%H:%M:%S%z ";
# log_theme = "default";
# term_charset = "utf-8";

import argparse
import sys
import re
import datetime
import json
import sqlite3

use_notice_format = True
nyt_soi_notice_format = r'^-{bot_name}:{channel}- [n|N]yt soi: (.+?)(?: - (.+?))?[\s]*$'
nyt_soi_privmsg_format = r'^<.{bot_name}> [n|N]yt soi: (.+?)(?: - (.+?))?[\s]*$'
nyt_esiintyy_format = r'^-{bot_name}:{channel}- [n|N]yt esiintyy: (.+?)[\s]*$' # not in use
tz = datetime.timezone.utc

def adapt_datetime(val):
    return val.strftime('%Y-%m-%d %H:%M:%S') # it is assumed datetimes are in utc

sqlite3.register_adapter(datetime.datetime, adapt_datetime)

class Track:
    def __init__(self, title, played_at, artist=None):
        self.__title = title
        self.__played_at = played_at
        self.__artist = artist

    def artist(self):
        return self.__artist

    def title(self):
        return self.__title

    def played_at(self):
        return self.__played_at

    def __str__(self):
        return '{playedAt:%x %X}: {artist} - {title}'.format(
            playedAt=self.__played_at, artist=self.__artist, title=self.__title)

    def __json__(self):
        return {'playedAt': self.__played_at.isoformat(), 'artist': self.__artist, 'title': self.__title}

class Parser:
    def __init__(self):
        self.__bot_name = 'ShoutBot'
        self.__channel = '#wappuradio'

        if use_notice_format:
            self.__pattern_nyt_soi = re.compile(nyt_soi_notice_format.format(
                bot_name=self.__bot_name,
                channel=self.__channel))
        else:
            self.__pattern_nyt_soi = re.compile(nyt_soi_privmsg_format.format(bot_name=self.__bot_name))

    def parse_input(self, input_p, callback=None, c_args=()):
        tracks = []

        if isinstance(input_p, list) and len(input_p) > 0:
            input_p = input_p[0]

        while True:
            try:
                line = input_p.readline()
            except KeyboardInterrupt:
                break

            if not line:
                break

            track = self.__parse_line(line.rstrip())
            if track:
                tracks.append(track)
                if callback:
                    callback(track, *c_args)

        return tracks

    def __parse_line(self, line):
        if not line:
            return None

        if line.startswith('---'):
            return None
        else:
            parsed_line = line.split(' ', 1)  # <timestamp> <rest of the message>
            if len(parsed_line) < 2:
                return None

            result = self.__pattern_nyt_soi.match(parsed_line[1])  # 1: artist (or title if artist is missing), 2: title
            if result:
                timestamp = datetime.datetime.fromisoformat(parsed_line[0]) # assumes log timestamps are in iso format
                if result.group(2):
                    return Track(artist=result.group(1), title=result.group(2), played_at=timestamp)
                else:
                    return Track(title=result.group(1), played_at=timestamp)  # artist/title is missing
        return None

class JSONTrackEncoder(json.JSONEncoder):
    def default(self, obj):
        if hasattr(obj, '__json__'):
            return obj.__json__()
        return super().default(obj)

def db_write(track, conn, table_name):
    if not track:
        return

    utcnow = datetime.datetime.now(tz)

    print('writing track to database: {track}'.format(track=track))

    try:
        with conn:
            conn.execute('INSERT INTO {table} ("artist", "title", "playedAt", "createdAt", "updatedAt")'
                         'VALUES (:artist, :title, :playedAt, :createdAt, :updatedAt)'.format(table=table_name),
                         {
                             "artist": track.artist(),
                             "title": track.title(),
                             "playedAt": track.played_at().astimezone(tz),
                             "createdAt": utcnow,
                             "updatedAt": utcnow
                         })
    except sqlite3.Error as e:
        print('transaction failed: {error}'.format(error=str(e)), file=sys.stderr)
    except UnicodeEncodeError as e:
        print('encoding failed: {error}'.format(error=str(e)), file=sys.stderr)

def db_write_many(tracks, conn, table_name):
    if len(tracks) == 0:
        return

    utcnow = datetime.datetime.now(tz)

    tracks_data = map(lambda track: (
        track.artist(), track.title(), track.played_at().astimezone(tz), utcnow, utcnow), tracks)

    print('writing {num_tracks} tracks to database'.format(num_tracks=len(tracks)))

    try:
        with conn:
            conn.executemany('INSERT INTO {table} ("artist", "title", "playedAt", "createdAt", "updatedAt")'
                             'VALUES (:artist, :title, :playedAt, :createdAt, :updatedAt)'
                             .format(table=table_name),
                             tracks_data)
    except sqlite3.Error as e:
        print('transaction failed: {error}'.format(error=str(e)), file=sys.stderr)
    except UnicodeEncodeError as e:
        print('encoding failed: {error}'.format(error=str(e)), file=sys.stderr)

def main():
    argparser = argparse.ArgumentParser(description='irssi log parser for wappu-api', allow_abbrev=False)
    argparser.add_argument(
        'input_log',
        metavar='INPUT_LOG',
        nargs='?',
        type=argparse.FileType('r', encoding='utf-8', errors='replace'),
        default=sys.stdin,
        help='the log file to parse, defaults to stdin')

    output_group = argparser.add_mutually_exclusive_group(required=False)
    output_group.add_argument(
        '--json-out', '-o',
        type=argparse.FileType('w', encoding='utf-8', errors='replace'),
        help='output as JSON to the specified file')
    output_group.add_argument(
        '--sqlite-db', '-d',
        type=str,
        help='writes to the database given')

    argparser.add_argument(
        '--sqlite-table', '-t',
        type=str,
        default='Tracks',
        help='table for SQL, defaults to \'Tracks\'')
    argparser.add_argument(
        '--immediate', '-i',
        action='store_true',
        help='handle every track immediately (not JSON)')

    args = argparser.parse_args()
    if not args.input_log:
        print('error: no input given', file=sys.stderr)
        exit(1)

    parser = Parser()

    if args.sqlite_db:
        connection = sqlite3.connect(args.sqlite_db)
        if args.immediate:
            parser.parse_input(args.input_log, db_write, (connection, args.sqlite_table))
        else:
            tracks = parser.parse_input(args.input_log)
            db_write_many(tracks, connection, args.sqlite_table)
        connection.close()
    elif args.json_out:
        tracks = parser.parse_input(args.input_log)
        print('writing JSON to \'{file_name}\''.format(file_name=args.json_out.name))
        encoder = JSONTrackEncoder(ensure_ascii=False, indent=2)
        result = encoder.encode(tracks)
        args.json_out.write(result)
    else:
        if args.immediate:
            parser.parse_input(args.input_log, lambda x: print(x))
        else:
            tracks = parser.parse_input(args.input_log)
            for track in tracks:
                print(track)

