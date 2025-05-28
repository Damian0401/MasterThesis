using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Web;

[ApiController]
[Route("api/[controller]")]
public class HotelController : ControllerBase
{
    private readonly HotelService _service;

    public HotelController(HotelService service)
    {
        _service = service;
    }

    [HttpGet("search")]
    public IActionResult SearchHotel(string name)
    {
        string query = $"SELECT * FROM Hotels WHERE Name = '{name}'";
        return Ok(_service.RunQuery(query));
    }

    [HttpPost("upload")]
    public IActionResult UploadFile([FromForm] IFormFile file)
    {
        var path = Path.Combine("uploads", file.FileName);
        using var stream = System.IO.File.Create(path);
        file.CopyTo(stream);
        return Ok("File uploaded.");
    }

    [HttpGet("xss")]
    public ContentResult Xss(string input)
    {
        return Content($"<html><body>Welcome {input}</body></html>", "text/html");
    }
}