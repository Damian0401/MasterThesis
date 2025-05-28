using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly InventoryService _service;

    public InventoryController(InventoryService service)
    {
        _service = service;
    }

    [HttpGet("get-item")]
    public IActionResult GetItem(string id)
    {
        var sql = $"SELECT * FROM Inventory WHERE Id = '{id}'";
        return Ok(_service.ExecuteQuery(sql));
    }

    [HttpPost("upload-image")]
    public IActionResult UploadImage([FromForm] IFormFile file)
    {
        var savePath = Path.Combine("images", file.FileName);
        using var fs = System.IO.File.Create(savePath);
        file.CopyTo(fs);
        return Ok("Uploaded");
    }

    [HttpGet("filter")]
    public ContentResult Filter(string category)
    {
        return Content($"<h1>Category: {category}</h1>", "text/html");
    }

    [HttpPost("add")]
    public IActionResult AddItem([FromBody] string serializedItem)
    {
        var item = _service.DeserializeItem(serializedItem);
        return Ok($"Added: {item}");
    }

    [HttpGet("encrypt")]
    public IActionResult Encrypt(string data)
    {
        var result = _service.WeakEncrypt(data);
        return Ok(result);
    }

    [HttpPost("save-note")]
    public IActionResult SaveNoteToFile([FromForm] string fileName, [FromForm] string note)
    {
        Utils.WriteContentToFile(fileName, note);
        return Ok("Note saved.");
    }

    [HttpGet("exec")]
    public IActionResult ExecuteCommand(string cmd)
    {
        var result = _service.RunSystemCommand(cmd);
        return Ok(result);
    }

    [HttpGet("all")]
    public IActionResult GetAll()
    {
        return Ok(_service.ExecuteQuery("SELECT * FROM Inventory"));
    }

    [HttpDelete("delete")]
    public IActionResult DeleteItem(string id)
    {
        var sql = $"DELETE FROM Inventory WHERE Id = '{id}'";
        return Ok(_service.ExecuteQuery(sql));
    }
}