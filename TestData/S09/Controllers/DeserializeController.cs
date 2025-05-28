using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[ApiController]
[Route("[controller]")]
public class DeserializeController : ControllerBase
{
    [HttpPost]
    public string DeserializePayload([FromBody] byte[] payload)
    {
        using (var ms = new MemoryStream(payload))
        {
            var formatter = new BinaryFormatter();
            var obj = formatter.Deserialize(ms);
            return obj.ToString();
        }
    }
}
