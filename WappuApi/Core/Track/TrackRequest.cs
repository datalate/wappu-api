namespace WappuApi.Core.Track;

public record TrackRequest
{
    public string? Artist { get; init; }

    public required string Title { get; init; }

    public required DateTime PlayedAt { get; init; }
}
