using Microsoft.AspNetCore.Mvc;
using Models;

namespace TimelineService;

[Route("api/[controller]")]
public class TimelineController : Controller
{
    [HttpGet("Get10PostsForTimeline")]
    public async Task<ActionResult<Timeline>> Get10PostForTimeline()
    {
        var timeline = await _timelineService.Get10();

        if (timeline is null)
            return BadRequest();

        return Ok(timeline);
    }

    private readonly Services.TimelineService _timelineService;

    public TimelineController(Services.TimelineService timelineService)
    {
        _timelineService = timelineService;
    }
}