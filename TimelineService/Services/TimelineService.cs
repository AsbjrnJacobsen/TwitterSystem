using PostsService;
using TimelineService.Models;

namespace TimelineService.Services;

public class TimelineService
{
    public Timeline Get10()
    {
        List<Post> posts = _context.Posts
            .OrderBy(p => p.timestamp)
            .Take(10)
            .ToList();
            
        var returnObj = new Timeline() { Tweets = posts };
            
        return returnObj;
    }

    private readonly TSDBContext _context;
    public TimelineService(TSDBContext context)
    {
        _context = context;
    }
}