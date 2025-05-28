using Microsoft.AspNetCore.Mvc;
using MusicApp.Services;

namespace MusicApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpPost("login")]
    public IActionResult Login(string username, string password)
    {
        if (_userService.Authenticate(username, password))
            return Ok("Login successful");
        return Unauthorized();
    }

    [HttpGet("greet")]
    public IActionResult Greet(string name)
    {
        return Content($"<h1>Welcome, {name}</h1>", "text/html");
    }
}