using Microsoft.AspNetCore.Mvc;

namespace TimelineService;
[Route("api/[controller]")]
public class TimelineController : Controller
{
    // GET
    
    [HttpGet("Get10PostsForTimeline")]
    public IActionResult Get10PostForTimeline()
    {
        return Ok(_timelineService.Get10());
    }

    private readonly Services.TimelineService _timelineService;
    public TimelineController(Services.TimelineService timelineService)
    {
        _timelineService = timelineService;
    }
    
    
}