using System.Linq.Expressions;

namespace WappuApi.Core.Track;

public record TrackResponse : TrackRequest
{
    public int Id { get; init; }

    public static readonly Expression<Func<TrackEntity, TrackResponse>> Projection = (entity) => new()
    {
        Id = entity.Id,
        Artist = entity.Artist,
        Title = entity.Title,
        PlayedAt = entity.PlayedAt,
    };
}
