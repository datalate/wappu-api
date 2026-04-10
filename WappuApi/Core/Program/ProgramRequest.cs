using System.ComponentModel.DataAnnotations;

namespace WappuApi.Core.Program;

public record ProgramRequest
{
    [Required]
    public string Title { get; init; } = "";

    [Required]
    public DateTime StartAt { get; init; }

    [Required]
    public DateTime EndAt { get; init; }
}
