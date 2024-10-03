using Microsoft.AspNetCore.Mvc;
using Models;
namespace PostsService;
[ApiController]
[Route("api/[controller]")]
public class PostController : Controller
{
    private readonly PSDBContext _psdbContext;
    public PostController(PSDBContext psdbContext)
    {
        _psdbContext = psdbContext;
    }

    [HttpPost("PostTweet")]
    public IActionResult PostTweet([FromBody] Post post)
    {
        _psdbContext.Posts.Add(post);
        _psdbContext.SaveChanges();
        return Ok(post);
    }
}