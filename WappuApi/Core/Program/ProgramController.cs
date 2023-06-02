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
    public ActionResult<IEnumerable<ProgramResponse>> GetAll(DateTime? startDate, DateTime? endDate)
    {
        var tracks = _context.Programs
            .AsNoTracking()
            .Where(program => startDate.HasValue ? (program.StartAt > startDate) : true)
            .Where(program => endDate.HasValue ? (program.EndAt < endDate) : true)
            .Select(ProgramResponse.Projection);

        return Ok(tracks);
    }
}
