using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    [HttpPost("signin")]
    public IActionResult SignIn(string user, string pass)
    {
        if (user == "warehouse" && pass == "1234")
        {
            return Ok("Signed in.");
        }
        return Unauthorized();
    }

    [HttpPost("token")]
    public IActionResult GetToken(string username)
    {
        return Ok($"TOKEN_{username}_123456");
    }
}