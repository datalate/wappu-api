using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WappuApi.Core.Track;

[ApiController]
[Route("tracks")]
public class TrackController : ControllerBase
{
    private readonly ILogger<TrackController> _logger;
    private readonly DataContext _context;

    public TrackController(
        ILogger<TrackController> logger,
        DataContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet()]
    public ActionResult<IEnumerable<TrackResponse>> GetAll(DateTime? startDate, DateTime? endDate)
    {
        var tracks = _context.Tracks
            .AsNoTracking()
            .Where(track => startDate.HasValue ? (track.PlayedAt > startDate) : true)
            .Where(track => endDate.HasValue ? (track.PlayedAt < endDate) : true)
            .Select(TrackResponse.Projection);

        return Ok(tracks);
    }
}
