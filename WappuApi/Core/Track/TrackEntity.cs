namespace WappuApi.Core.Track;

public class TrackEntity : EntityBase
{
    public TrackEntity() { }

    public string? Artist { get; set; }
    public string Title { get; set; } = null!;
    public DateTime PlayedAt { get; set; }
}
