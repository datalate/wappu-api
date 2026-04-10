using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WappuApi.Core.Program;

namespace WappuApi.Core.Import;

[ApiController]
[Route("import")]
[Authorize]
public class ImportController(
    ILogger<ImportController> logger,
    IHttpClientFactory httpClientFactory,
    DataContext context) : ControllerBase
{
    [HttpPost("")]
    public async Task<ActionResult> Import()
    {
        var client = httpClientFactory.CreateClient("Import");
        using var response = await client.GetAsync("/api/programs");
        response.EnsureSuccessStatusCode();

        var programs = (await response.Content.ReadFromJsonAsync<IEnumerable<ImportedProgramDto>>() ?? []).ToList();

        logger.LogInformation("Importing {Count} programs", programs.Count);

        context.Programs.AddRange(
            programs.Select(program => new ProgramEntity
            {
                Title = program.Title,
                StartAt = program.Start,
                EndAt = program.End,
            })
        );

        await context.SaveChangesAsync();

        return Ok();
    }
}
