using System.ComponentModel.DataAnnotations;

namespace WappuApi.Core.Program;

public record ProgramRequest
{
    public ProgramRequest() { }

    public string Title { get; init; } = null!;
    [Required]
    public DateTime? StartAt { get; init; }
    [Required]
    public DateTime? EndAt { get; init; }
}
