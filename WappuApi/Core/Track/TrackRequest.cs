namespace WappuApi.Core.Track;

public record TrackRequest
{
    public string? Artist { get; init; }
    public string Title { get; init; } = null!;
    public DateTime PlayedAt { get; init; }
}
