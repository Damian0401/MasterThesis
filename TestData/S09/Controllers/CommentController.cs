using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class CommentController : ControllerBase
{
    [HttpPost]
    public string PostComment([FromBody] string comment)
    {
        return $"<html><body>Your comment: {comment}</body></html>";
    }
}
