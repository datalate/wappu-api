using Microsoft.AspNetCore.Mvc;
using WappuApi.Core.Program;

namespace WappuApi.Core.Import;

[ApiController]
[Route("import")]
public class ImportController(
    ILogger<ImportController> logger,
    DataContext context) : ControllerBase
{
    private readonly ILogger<ImportController> _logger = logger;
    private readonly DataContext _context = context;

    private static readonly SocketsHttpHandler handler = new()
    {
        PooledConnectionLifetime = TimeSpan.FromMinutes(15) // Recreate every 15 minutes
    };

    private static readonly HttpClient httpClient = new(handler)
    {
        BaseAddress = new Uri("https://wappuradio.fi"),
    };

    [HttpPost("")]
    public async Task<ActionResult> Import()
    {
        using var response = await httpClient.GetAsync("/api/programs");
        response.EnsureSuccessStatusCode();

        var programs = (await response.Content.ReadFromJsonAsync<IEnumerable<ImportedProgramDto>>() ?? []).ToList();

        _logger.LogInformation("Importing {Count} programs", programs.Count);

        _context.Programs.AddRange(
            programs.Select(program => new ProgramEntity
            {
                Title = program.Title,
                StartAt = program.Start,
                EndAt = program.End,
            })
        );

        await _context.SaveChangesAsync();

        return Ok();
    }
}
