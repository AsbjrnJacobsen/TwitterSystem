using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Models;
using Polly;
using Polly.Retry;

namespace GatewayAPI;

[Route("api/[controller]")]
[ApiController]
public class APIController : Controller
{
    private RetryPollyLayer _retryLayer;
    public APIController(IConfiguration configuration, RetryPollyLayer retryLayer)
    {
        _configuration = configuration;
        _retryLayer = retryLayer;
    }

    private readonly IConfiguration _configuration;

    [HttpPost("createNewUser")]
    public async Task<IActionResult> CreateNewUser([FromBody] Account createUserRequest)
    {
        // Http request to the AccountService, to check if the username exists as an Account already.
        //var client = new HttpClient();
        //client.BaseAddress = new Uri(_configuration["AccountServiceUrl"]);
        //var res = await client.GetAsync("api/Account/GetAccountByName/" + createUserRequest.Username).Result.Content
        //    .ReadAsStringAsync();
        var endpointUrl = new Uri(_configuration["AccountServiceUrl"] + 
                                "api/Account/GetAccountByName/" +
                                createUserRequest.Username);
        var res = await _retryLayer.GetAsyncWithRetry(endpointUrl);
        var content = await res.Content.ReadAsStringAsync();
        var account = content != String.Empty ? JsonSerializer.Deserialize<Account>(content) : null;
        
        // Check if accounts returned is the same as the one supplied in the request
        if (account is not null && account.Username == createUserRequest.Username) return BadRequest();
        
        // Create account
        //client.PostAsync("api/Account/CreateAccount/",
        //        new StringContent(JsonSerializer.Serialize(createUserRequest), Encoding.UTF8, "application/json"))
        //    .Result
        //    .EnsureSuccessStatusCode();
        
        var postEndpointUrl = new Uri(_configuration["AccountServiceUrl"] + "api/Account/CreateAccount"); 
        var postContent = new StringContent(JsonSerializer.Serialize(createUserRequest), Encoding.UTF8, "application/json");
        (await _retryLayer.PostAsyncWithRetry(postEndpointUrl, postContent)).EnsureSuccessStatusCode();
        
        return Ok();
    }

    [HttpGet("Get10Posts")]
    public async Task<ActionResult<Timeline>> Get10Posts()
    {
        var client = new HttpClient();
        client.BaseAddress = new Uri(_configuration["TimelineServiceUrl"]);
        var response = await client.GetAsync("api/Timeline/Get10PostsForTimeline");
        var res = await response.Content.ReadAsStringAsync();
        var timeline = JsonSerializer.Deserialize<Timeline>(res, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
        if (timeline is not null) 
            return Ok(timeline);
        
        return BadRequest();
    }

    [HttpPost("PostTweet")]
    public async Task<IActionResult> PostTweet([FromBody] Post post)
    {
        var client = new HttpClient();
        client.BaseAddress = new Uri(_configuration["PostServiceUrl"]);
        var json = JsonSerializer.Serialize(post);
        var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
        var res = await client.PostAsync("api/Post/PostTweet", stringContent).Result.Content.ReadAsStringAsync();
        var resPost = JsonSerializer.Deserialize<Post>(res);
        if (post is not null) return Ok(resPost);
        return BadRequest();
    }
}