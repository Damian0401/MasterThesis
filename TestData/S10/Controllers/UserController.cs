using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserAccountService _userService;

    public UserController(UserAccountService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterUserRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = _userService.Register(request);
        return result ? Ok("User registered.") : Conflict("User already exists.");
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = _userService.Login(request);
        return result ? Ok("Login successful.") : Unauthorized("Invalid credentials.");
    }
}