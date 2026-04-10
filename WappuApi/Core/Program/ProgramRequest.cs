using System.ComponentModel.DataAnnotations;

namespace WappuApi.Core.Program;

public record ProgramRequest : IValidatableObject
{
    public required string Title { get; init; }

    public required DateTime StartAt { get; init; }

    public required DateTime EndAt { get; init; }

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
