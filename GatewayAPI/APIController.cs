using System.Net;
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
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    public APIController(IConfiguration configuration, RetryPollyLayer retryLayer)
    {
        _configuration = configuration;
        _retryLayer = retryLayer;

        _jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    private readonly IConfiguration _configuration;

    [HttpPost("createNewUser")]
    public async Task<IActionResult> CreateNewUser([FromBody] Account createUserRequest)
    {
        // Http request to the AccountService, to check if the username exists as an Account already.
        var endpointUrl = new Uri(_configuration["AccountServiceUrl"] + "api/Account/GetAccountByName/" + 
                                  createUserRequest.Username);
        var res = await _retryLayer
            .GetAsyncWithRetry(
                endpointUrl, 
                HttpStatusCode.NotFound, 
                HttpStatusCode.OK);
        
        // If username is found, no account can be created
        if (res.StatusCode == HttpStatusCode.OK) 
            return BadRequest();
        
        // Create account if username is not found
        var postEndpointUrl = new Uri(_configuration["AccountServiceUrl"] + "api/Account/CreateAccount"); 
        var postContent = new StringContent(JsonSerializer.Serialize(createUserRequest), Encoding.UTF8, 
            "application/json");
        (await _retryLayer
                .PostAsyncWithRetry(
                    postEndpointUrl, 
                    postContent,
                    HttpStatusCode.OK)
            ).EnsureSuccessStatusCode();
        return Ok();
    }

    [HttpGet("Get10Posts")]
    public async Task<ActionResult<Timeline>> Get10Posts()
    {
        // Call timeline service for timeline with 10 posts
        var endpointUrl = new Uri(_configuration["TimelineServiceUrl"] + "api/Timeline/Get10PostsForTimeline");
        var httpRes = await _retryLayer.GetAsyncWithRetry(endpointUrl, HttpStatusCode.OK, HttpStatusCode.BadRequest);

        // If OK
        if (httpRes.StatusCode == HttpStatusCode.OK) 
        {
            // Read response as string
            var res = await httpRes.Content.ReadAsStringAsync();
            // Deserialize to timeline object
            var timeline = JsonSerializer.Deserialize<Timeline>(res, _jsonSerializerOptions);
        
            // If not null return timeline
            if (timeline is not null) 
                return Ok(timeline);    
        }
        
        return BadRequest();
    }

    [HttpPost("PostTweet")]
    public async Task<IActionResult> PostTweet([FromBody] Post post)
    {
        // Serialize post to json for request
        var json = JsonSerializer.Serialize(post);
        var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
        
        // Send the request
        var endpointUrl = new Uri(_configuration["PostServiceUrl"] + "api/Post/PostTweet");
        var httpRes = await _retryLayer.PostAsyncWithRetry(endpointUrl, stringContent, HttpStatusCode.OK);

        if (httpRes.StatusCode == HttpStatusCode.OK)
        {
            // Read content as string
            var res = await httpRes.Content.ReadAsStringAsync();
        
            // Deserialize string into Post object
            var resPost = JsonSerializer.Deserialize<Post>(res);
            
            // Return Post
            if (post is not null) 
                return Ok(resPost);    
        }
        
        return BadRequest();
    }
}