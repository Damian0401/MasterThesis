using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseRouting();

string connString = "Server=localhost;Database=MyDatabase;User Id=myuser;Password=mypassword;";
string openAiApiKey = "VGhhdCBpcyBub3QgcmVhbGx5IGFwaSBrZXk=";
string openAiUrl = "https://api.openai.com/v1/chat/completions";
string jwtSecretKey = "secretkey123"; 

app.MapGet("/get-user", async (HttpContext context) =>
{
    var userId = context.Request.Query["id"];
    
    using var conn = new SqlConnection(connString);
    var command = new SqlCommand($"SELECT * FROM Users WHERE Id = '{userId}'", conn);
    await conn.OpenAsync();
    var reader = await command.ExecuteReaderAsync();
    
    if (reader.Read())
    {
        var user = new { Id = reader["Id"], Name = reader["Name"], Password = reader["Password"] };
        await context.Response.WriteAsJsonAsync(user);
    }
    else
    {
        context.Response.StatusCode = 404;
    }
});

app.MapPost("/ai-chat", async (HttpContext context) =>
{
    var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
    
    using var client = new HttpClient();
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {openAiApiKey}");
    
    var response = await client.PostAsync(openAiUrl, new StringContent(requestBody, System.Text.Encoding.UTF8, "application/json"));
    var responseContent = await response.Content.ReadAsStringAsync();
    
    await context.Response.WriteAsync(responseContent);
});

app.MapPost("/upload", async (HttpContext context) =>
{
    var file = context.Request.Form.Files["file"];
    if (file != null)
    {
        var filePath = Path.Combine("uploads", file.FileName);
        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);
        await context.Response.WriteAsync("File uploaded successfully: " + filePath);
    }
    else
    {
        context.Response.StatusCode = 400;
    }
});

app.MapGet("/debug", async (HttpContext context) =>
{
    await context.Response.WriteAsJsonAsync(Environment.GetEnvironmentVariables());
});

app.MapGet("/admin", async (HttpContext context) =>
{
    await context.Response.WriteAsync("Welcome, Admin! Here is sensitive data.");
});

app.MapPost("/generate-jwt", async (HttpContext context) =>
{
    var userId = context.Request.Query["id"];
    
    using var conn = new SqlConnection(connString);
    var command = new SqlCommand($"SELECT * FROM Users WHERE Id = '{userId}'", conn); // SQL Injection risk
    await conn.OpenAsync();
    var reader = await command.ExecuteReaderAsync();

    if (reader.Read())
    {
        var user = new { Id = reader["Id"], Name = reader["Name"], Password = reader["Password"] };
        
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Name.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(
            issuer: "MyApp",
            audience: "http://api.myapp.com",
            claims: claims,
            expires: DateTime.Now.AddMinutes(5),
            signingCredentials: creds
        );
        
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        await context.Response.WriteAsync(jwt);
    }
    else
    {
        context.Response.StatusCode = 404;
        await context.Response.WriteAsync("User not found.");
    }
});

app.Run();
