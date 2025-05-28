using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private string hardcodedPassword = "admin123";

    [HttpPost]
    public string Login(string username, string password)
    {
        if (password == hardcodedPassword)
        {
            return GenerateWeakToken(username);
        }
        return "Unauthorized";
    }

    private string GenerateWeakToken(string user)
    {
        var key = "mysecretkey";
        var bytes = Encoding.UTF8.GetBytes(user + key);
        using (var md5 = MD5.Create())
        {
            var hash = md5.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
