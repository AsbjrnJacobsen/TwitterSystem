using System.Text;
using System.Text.Json;
using AccountService;
using Microsoft.AspNetCore.Mvc;

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
        HttpClient client = new HttpClient();
        client.BaseAddress = new Uri(_configuration["AccountServiceUrl"]);
        var res = await client.GetAsync("api/Account/GetAccountByName/" + createUserRequest.Username).Result.Content.ReadAsStringAsync();
        Account account = JsonSerializer.Deserialize<Account>(res);

        if (account.Username == createUserRequest.Username)
        {
            return BadRequest();
        }
        else
        {
            client.PostAsync("api/Account/CreateAccount", new StringContent(createUserRequest.Password, Encoding.UTF8, "application/json")).Result.EnsureSuccessStatusCode();
        }
        
        return Ok(); 
    }
    
}