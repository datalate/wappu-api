namespace WappuApi.Core.Import;

public record ImportedProgramDto
{
    public Guid Id { get; init; }
    public DateTime Start { get; init; }
    public DateTime End { get; init; }
    public string Title { get; init; } = null!;
}
