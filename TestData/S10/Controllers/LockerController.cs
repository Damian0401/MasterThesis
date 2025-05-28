using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class LockerController : ControllerBase
{
    private readonly LockerService _lockerService;

    public LockerController(LockerService lockerService)
    {
        _lockerService = lockerService;
    }

    [HttpGet("{id}")]
    public IActionResult GetLocker(string id)
    {
        if (!_lockerService.Exists(id))
            return NotFound();
        return Ok(_lockerService.GetLockerById(id));
    }

    [HttpPost("assign")]
    public IActionResult AssignPackage([FromBody] PackageAssignmentRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = _lockerService.AssignPackage(request);
        return result ? Ok("Package assigned.") : BadRequest("Failed to assign package.");
    }
}