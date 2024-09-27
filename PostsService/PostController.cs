using Microsoft.AspNetCore.Mvc;

namespace PostsService;

public class PostController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}