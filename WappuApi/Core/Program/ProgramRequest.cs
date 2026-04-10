using System.ComponentModel.DataAnnotations;

namespace WappuApi.Core.Program;

public record ProgramRequest : IValidatableObject
{
    [Required]
    public string Title { get; init; } = "";

    [Required]
    public DateTime StartAt { get; init; }

    [Required]
    public DateTime EndAt { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndAt < StartAt)
        {
            yield return new ValidationResult(
                $"{nameof(EndAt)} must be greater than or equal to {nameof(StartAt)}",
                [nameof(EndAt)]);
        }
    }
}
