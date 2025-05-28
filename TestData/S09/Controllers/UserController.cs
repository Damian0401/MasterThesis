using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    [HttpGet]
    public string GetUser(string username)
    {
        var conn = new SqlConnection("Data Source=localhost;Initial Catalog=TestDb;Integrated Security=True");
        conn.Open();
        var cmd = new SqlCommand($"SELECT * FROM Users WHERE Username = '{username}'", conn);
        var reader = cmd.ExecuteReader();
        return reader.HasRows ? "User found" : "User not found";
    }
}
