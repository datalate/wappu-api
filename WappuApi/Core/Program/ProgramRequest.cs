namespace WappuApi.Core.Program;

public record ProgramRequest
{
    public string Title { get; init; } = null!;
    public DateTime StartAt { get; init; }
    public DateTime EndAt { get; init; }
}
