using System.ComponentModel.DataAnnotations;

namespace WappuApi.Core.Track;

public record TrackRequest
{
    public TrackRequest() { }

    public string? Artist { get; init; }
    [Required]
    public string Title { get; init; } = null!;
    [Required]
    public DateTime? PlayedAt { get; init; }
}
