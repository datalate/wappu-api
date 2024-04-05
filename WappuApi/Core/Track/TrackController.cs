using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WappuApi.Core.Track;

[ApiController]
[Route("tracks")]
public class TrackController(
    ILogger<TrackController> logger,
    DataContext context) : ControllerBase
{
    private readonly ILogger<TrackController> _logger = logger;
    private readonly DataContext _context = context;

    [HttpGet()]
    public ActionResult<IEnumerable<TrackResponse>> GetAll([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var tracks = _context.Tracks
            .AsNoTracking()
            .Where(track => startDate.HasValue ? (track.PlayedAt > startDate) : true)
            .Where(track => endDate.HasValue ? (track.PlayedAt < endDate) : true)
            .Select(TrackResponse.Projection);

        return Ok(tracks);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TrackResponse>> Get([FromRoute] int id)
    {
        var track = await _context.Tracks
            .AsNoTracking()
            .SingleOrDefaultAsync(track => track.Id == id);

        if (track == default)
            return NotFound();

        return Ok(TrackResponse.Projection.Compile().Invoke(track));
    }

    [HttpPost]
    public async Task<ActionResult<TrackResponse>> Post([FromBody] TrackRequest request)
    {
        var track = _context.Tracks.Add(new TrackEntity()).Entity;
        track = MapFields(track, request);

        await _context.SaveChangesAsync();

        return Ok(TrackResponse.Projection.Compile().Invoke(track));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TrackResponse>> Put([FromBody] TrackRequest request, [FromRoute] int id)
    {
        var track = await _context.Tracks
            .SingleOrDefaultAsync(track => track.Id == id);

        if (track == default)
            return NotFound();

        track = MapFields(track, request);

        await _context.SaveChangesAsync();

        return Ok(TrackResponse.Projection.Compile().Invoke(track));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete([FromRoute] int id)
    {
        var track = await _context.Tracks
            .SingleOrDefaultAsync(track => track.Id == id);

        if (track == default)
            return NotFound();

        _context.Remove(track);

        await _context.SaveChangesAsync();

        return Ok();
    }

    private TrackEntity MapFields(TrackEntity track, TrackRequest request)
    {
        track.Artist = request.Artist;
        track.Title = request.Title;
        track.PlayedAt = request.PlayedAt!.Value;

        return track;
    }
}
