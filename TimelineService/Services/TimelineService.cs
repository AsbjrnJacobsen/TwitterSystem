using Models;

namespace TimelineService.Services;

public class TimelineService
{
    public Timeline Get10()
    {
        List<Post> posts = _psdbContext.Posts
            .OrderBy(p => p.timestamp)
            .Take(10)
            .ToList();
            
        var returnObj = new Timeline() { Tweets = posts };
            
        return returnObj;
    }

    private readonly ASDBContext _asdbContext;
    private readonly PSDBContext _psdbContext;
    public TimelineService(ASDBContext asdbContext, PSDBContext psdbContext)
    {
        _asdbContext = asdbContext;
        _psdbContext = psdbContext;
    }
}