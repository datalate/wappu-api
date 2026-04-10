using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WappuApi.Core.Track;

[ApiController]
[Route("tracks")]
public class TrackController(
    ILogger<TrackController> logger,
    DataContext context) : ControllerBase
{
    [HttpGet("")]
    public async Task<ActionResult<IEnumerable<TrackResponse>>> GetAll([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var tracks = await context.Tracks
            .AsNoTracking()
            .Where(track => !startDate.HasValue || (track.PlayedAt > startDate))
            .Where(track => !endDate.HasValue || (track.PlayedAt < endDate))
            .ToListAsync();

        return Ok(tracks.Select(TrackResponse.Projection.Compile()));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TrackResponse>> Get([FromRoute] int id)
    {
        var track = await context.Tracks
            .AsNoTracking()
            .SingleOrDefaultAsync(track => track.Id == id);

        if (track == default)
            return NotFound();

        return Ok(TrackResponse.Projection.Compile().Invoke(track));
    }

    [HttpPost("")] 
    [Authorize]
    public async Task<ActionResult<TrackResponse>> Post([FromBody] TrackRequest request)
    {
        var track = context.Tracks.Add(new TrackEntity()).Entity;
        track = MapFields(track, request);

        await context.SaveChangesAsync();

        return Ok(TrackResponse.Projection.Compile().Invoke(track));
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<TrackResponse>> Put([FromBody] TrackRequest request, [FromRoute] int id)
    {
        var track = await context.Tracks
            .SingleOrDefaultAsync(track => track.Id == id);

        if (track == default)
            return NotFound();

        track = MapFields(track, request);

        await context.SaveChangesAsync();

        return Ok(TrackResponse.Projection.Compile().Invoke(track));
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult> Delete([FromRoute] int id)
    {
        var track = await context.Tracks
            .SingleOrDefaultAsync(track => track.Id == id);

        if (track != default)
        {
            context.Remove(track);
            await context.SaveChangesAsync();
        }

        return Ok();
    }

    [HttpDelete("")]
    [Authorize]
    public async Task<ActionResult> DeleteRange([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var tracks = await context.Tracks
            .Where(track => track.PlayedAt >= from && track.PlayedAt <= to)
            .ToListAsync();

        logger.LogInformation("Deleting {Count} tracks", tracks.Count);

        context.RemoveRange(tracks);
        await context.SaveChangesAsync();

        return Ok();
    }

    private static TrackEntity MapFields(TrackEntity track, TrackRequest request)
    {
        track.Artist = request.Artist;
        track.Title = request.Title;
        track.PlayedAt = request.PlayedAt;

        return track;
    }
}
