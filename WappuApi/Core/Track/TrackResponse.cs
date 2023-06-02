using System.Linq.Expressions;

namespace WappuApi.Core.Track;

public record TrackResponse
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
    public string? Artist { get; init; }
    public string Title { get; init; } = null!;
    public DateTime PlayedAt { get; init; }
}
