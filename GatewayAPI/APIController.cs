using System.Text;
using System.Text.Json;
using AccountService;
using Microsoft.AspNetCore.Mvc;
using PostsService;
using TimelineService.Models;

namespace GatewayAPI;

[Route("api/[controller]")]
[ApiController]
public class APIController : Controller
{
    public APIController(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    private readonly IConfiguration _configuration;

    [HttpPost("createNewUser")]
    public async Task<IActionResult> CreateNewUser([FromBody] Account createUserRequest)
    {
        // Http request to the AccountService, to check if the username exists as an Account already.
        HttpClient client = new HttpClient();
        client.BaseAddress = new Uri(_configuration["AccountServiceUrl"]);
        var res = await client.GetAsync("api/Account/GetAccountByName/" + createUserRequest.Username).Result.Content
            .ReadAsStringAsync();
        Account account = JsonSerializer.Deserialize<Account>(res);

        // Check if accounts returned is the same as the one supplied in the request
        if (account is not null && account.Username == createUserRequest.Username)
        {
            return BadRequest();
        }
        
        // Create account
        client.PostAsync("api/Account/CreateAccount",
                new StringContent(JsonSerializer.Serialize(createUserRequest), Encoding.UTF8, "application/json")).Result
           .EnsureSuccessStatusCode();
        return Ok();
    }

    [HttpGet("Get10Posts")]
    public async Task<IActionResult> Get10Posts()
    {
        HttpClient client = new HttpClient();
        client.BaseAddress = new Uri(_configuration["TimelineServiceUrl"]);
        var res = await client.GetAsync("api/Timeline/GetTimeline").Result.Content.ReadAsStringAsync();
        Timeline timeline = JsonSerializer.Deserialize<Timeline>(res);
        if (timeline is not null)
        {
            return Ok(timeline);
        }
        return BadRequest();
    }

    [HttpPost("PostTweet")]
    public async Task<IActionResult> PostTweet()
    {
        HttpClient client = new HttpClient();
        client.BaseAddress = new Uri(_configuration["PostServiceUrl"]);
        var res = await client.GetAsync("api/PostTweet").Result.Content.ReadAsStringAsync();
        Post post = JsonSerializer.Deserialize<Post>(res);
        if (post is not null)
        {
            return Ok(post);
        }
        return BadRequest();
    }
}