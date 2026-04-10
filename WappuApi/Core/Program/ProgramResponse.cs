using System.Linq.Expressions;

namespace WappuApi.Core.Program;

public record ProgramResponse : ProgramRequest
{
    public int Id { get; init; }

    private static readonly Expression<Func<ProgramEntity, ProgramResponse>> Projection = (entity) => new()
    {
        Id = entity.Id,
        Title = entity.Title,
        StartAt = entity.StartAt,
        EndAt = entity.EndAt,
    };

    public static readonly Func<ProgramEntity, ProgramResponse> Map = Projection.Compile();
}
