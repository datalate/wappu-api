using System.Linq.Expressions;

namespace WappuApi.Core.Track;

public record TrackResponse : TrackRequest
{
    public TrackResponse() { }

    public static Expression<Func<TrackEntity, TrackResponse>> Projection = (entity) => new TrackResponse()
    {
        Id = entity.Id,
        Artist = entity.Artist,
        Title = entity.Title,
        PlayedAt = entity.PlayedAt,
    };

    public int Id { get; init; }
}
