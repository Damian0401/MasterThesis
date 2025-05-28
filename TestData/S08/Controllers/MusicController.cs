using Microsoft.AspNetCore.Mvc;
using MusicApp.Services;

namespace MusicApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MusicController : ControllerBase
{
    private readonly MusicService _musicService;

    public MusicController(MusicService musicService)
    {
        _musicService = musicService;
    }

    [HttpGet("track")]
    public IActionResult GetTrack(string id)
    {
        return Ok(_musicService.GetTrackById(id));
    }

    [HttpPost("upload")]
    public IActionResult UploadTrack([FromForm] IFormFile file)
    {
        return Ok(_musicService.SaveTrackFile(file));
    }

    [HttpPost("play")]
    public IActionResult PlayTrack([FromBody] string trackData)
    {
        return Ok(_musicService.DeserializeTrack(trackData));
    }
}