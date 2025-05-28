using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using System.Text.Encodings.Web;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Inicjalizacja bazy danych SQLite w pamięci
var connectionString = "Data Source=:memory:";
var connection = new SqliteConnection(connectionString);
connection.Open();

// Tworzenie przykładowej tabeli
var command = connection.CreateCommand();
command.CommandText =
@"
CREATE TABLE Users (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT
);

INSERT INTO Users (Name) VALUES ('Alice');
INSERT INTO Users (Name) VALUES ('Bob');
";
command.ExecuteNonQuery();

app.MapGet("/", async context =>
{
    await context.Response.WriteAsync(@"
        <html>
        <body>
            <h1>Search Users</h1>
            <form method='get' action='/search'>
                <input name='name' />
                <button type='submit'>Search</button>
            </form>

            <h1>Say Hello</h1>
            <form method='get' action='/xss'>
                <input name='message' />
                <button type='submit'>Send</button>
            </form>
        </body>
        </html>
    ");
});

app.MapGet("/search", async context =>
{
    var name = context.Request.Query["name"];
    var sql = $"SELECT * FROM Users WHERE Name = '{name}'";
    var cmd = connection.CreateCommand();
    cmd.CommandText = sql;

    var reader = cmd.ExecuteReader();
    await context.Response.WriteAsync("<h2>Results:</h2><ul>");
    while (reader.Read())
    {
        await context.Response.WriteAsync($"<li>{reader["Name"]}</li>");
    }
    await context.Response.WriteAsync("</ul>");
});

app.MapGet("/template", async context =>
{
    var message = context.Request.Query["message"];
    await context.Response.WriteAsync($"<h2>Your message:</h2><p>{message}</p>");
});

app.Run();
