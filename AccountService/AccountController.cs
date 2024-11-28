using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
namespace AccountService;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AccountController : Controller
{
    private readonly ASDBContext _context;
    
    public AccountController(ASDBContext context)
    {
        _context = context;
    }

    [HttpGet("GetAccountByID/{id}")]
    public IActionResult GetAccountByID([FromQuery] int id)
    {
       Account accountFromQuery = _context.Accounts.Where(x => x.ID == id).First();
       return Ok(accountFromQuery);
    }

    [HttpGet("GetAccountByName/{name}")]
    public IActionResult GetAccountByName([FromRoute] string name)
    {
        // Check if name exists
        if (!_context.Accounts.Any(a => a.Username == name))
            return NotFound();
        
        // Get entry and return
        Account gottenAccountByName = _context.Accounts.First(x => x.Username == name);
        return Ok(gottenAccountByName);
    }
    
    [HttpPost("CreateAccount")]
    public IActionResult CreateAccount([FromBody] Account account)
    {
        _context.Accounts.Add(account);
        _context.SaveChanges();
        return Ok();
    }

    [HttpPut("UpdateAccount")]
    public IActionResult UpdateAccount([FromBody] Account account)
    {
        _context.Accounts.Update(account);
        _context.SaveChanges();
        return Ok();
    }

    [HttpDelete("DeleteAccount")]
    public IActionResult DeleteAccount([FromQuery] int id)
    {
        Account accountFromQuery = _context.Accounts.Where(x => x.ID == id).First();
        _context.Accounts.Remove(accountFromQuery);
        _context.SaveChanges();
        return Ok(accountFromQuery);
    }
}

