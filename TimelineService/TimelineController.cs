using Microsoft.AspNetCore.Mvc;

namespace TimelineService;

public class TimelineController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}