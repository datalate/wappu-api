using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WappuApi.Core.Program;

[ApiController]
[Route("programs")]
public class ProgramController : ControllerBase
{
    private readonly ILogger<ProgramController> _logger;
    private readonly DataContext _context;

    public ProgramController(
        ILogger<ProgramController> logger,
        DataContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet()]
    public ActionResult<IEnumerable<ProgramResponse>> GetAll([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var programs = _context.Programs
            .AsNoTracking()
            .Where(program => startDate.HasValue ? (program.StartAt > startDate) : true)
            .Where(program => endDate.HasValue ? (program.EndAt < endDate) : true)
            .Select(ProgramResponse.Projection);

        return Ok(programs);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProgramResponse>> Get([FromRoute] int id)
    {
        var program = await _context.Programs
            .AsNoTracking()
            .SingleOrDefaultAsync(program => program.Id == id);

        if (program == default)
            return NotFound();

        return Ok(ProgramResponse.Projection.Compile().Invoke(program));
    }

    [HttpPost]
    public async Task<ActionResult<ProgramResponse>> Post([FromBody] ProgramRequest request)
    {
        var program = _context.Programs.Add(new ProgramEntity()).Entity;
        program = MapFields(program, request);

        await _context.SaveChangesAsync();

        return Ok(ProgramResponse.Projection.Compile().Invoke(program));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProgramResponse>> Put([FromBody] ProgramRequest request, [FromRoute] int id)
    {
        var program = await _context.Programs
            .SingleOrDefaultAsync(program => program.Id == id);

        if (program == default)
            return NotFound();

        program = MapFields(program, request);

        await _context.SaveChangesAsync();

        return Ok(ProgramResponse.Projection.Compile().Invoke(program));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete([FromRoute] int id)
    {
        var program = await _context.Programs
            .SingleOrDefaultAsync(program => program.Id == id);

        if (program == default)
            return NotFound();

        _context.Remove(program);

        await _context.SaveChangesAsync();

        return Ok();
    }

    private ProgramEntity MapFields(ProgramEntity program, ProgramRequest request)
    {
        program.Title = request.Title;
        program.StartAt = request.StartAt!.Value;
        program.EndAt = request.EndAt!.Value;

        return program;
    }
}
