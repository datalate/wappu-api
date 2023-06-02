using System.Linq.Expressions;

namespace WappuApi.Core.Program;

public record ProgramResponse
{
    public ProgramResponse() { }

    public static Expression<Func<ProgramEntity, ProgramResponse>> Projection = (entity) => new ProgramResponse()
    {
        Id = entity.Id,
        Title = entity.Title,
        StartAt = entity.StartAt,
        EndAt = entity.EndAt,
    };

    public int Id { get; init; }
    public string Title { get; init; } = null!;
    public DateTime StartAt { get; init; }
    public DateTime EndAt { get; init; }
}
