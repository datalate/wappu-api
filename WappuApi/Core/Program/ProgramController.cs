using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WappuApi.Core.Program;

[ApiController]
[Route("programs")]
public class ProgramController(
    ILogger<ProgramController> logger,
    DataContext context) : ControllerBase
{
    [HttpGet("")]
    public async Task<ActionResult<IEnumerable<ProgramResponse>>> GetAll([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var programs = await context.Programs
            .AsNoTracking()
            .Where(program => !startDate.HasValue || (program.StartAt > startDate))
            .Where(program => !endDate.HasValue || (program.EndAt < endDate))
            .ToListAsync();

        return Ok(programs.Select(ProgramResponse.Projection.Compile()));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProgramResponse>> Get([FromRoute] int id)
    {
        var program = await context.Programs
            .AsNoTracking()
            .SingleOrDefaultAsync(program => program.Id == id);

        if (program == default)
            return NotFound();

        return Ok(ProgramResponse.Projection.Compile().Invoke(program));
    }

    [HttpPost("")]
    [Authorize]
    public async Task<ActionResult<ProgramResponse>> Post([FromBody] ProgramRequest request)
    {
        var program = context.Programs.Add(new ProgramEntity()).Entity;
        program = MapFields(program, request);

        await context.SaveChangesAsync();

        return Ok(ProgramResponse.Projection.Compile().Invoke(program));
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<ProgramResponse>> Put([FromBody] ProgramRequest request, [FromRoute] int id)
    {
        var program = await context.Programs
            .SingleOrDefaultAsync(program => program.Id == id);

        if (program == default)
            return NotFound();

        program = MapFields(program, request);

        await context.SaveChangesAsync();

        return Ok(ProgramResponse.Projection.Compile().Invoke(program));
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult> Delete([FromRoute] int id)
    {
        var program = await context.Programs
            .SingleOrDefaultAsync(program => program.Id == id);

        if (program != default) {
            context.Remove(program);
            await context.SaveChangesAsync();
        }

        return Ok();
    }

    [HttpDelete("")]
    [Authorize]
    public async Task<ActionResult> DeleteRange([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var programs = await context.Programs
            .Where(program => program.StartAt >= from && program.EndAt <= to)
            .ToListAsync();

        logger.LogInformation("Deleting {Count} programs", programs.Count);

        context.RemoveRange(programs);
        await context.SaveChangesAsync();

        return Ok();
    }

    private static ProgramEntity MapFields(ProgramEntity program, ProgramRequest request)
    {
        program.Title = request.Title;
        program.StartAt = request.StartAt;
        program.EndAt = request.EndAt;

        return program;
    }
}
