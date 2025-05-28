using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Models;

var builder = WebApplication.CreateBuilder(args);

// Konfiguracja EF Core z SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=users.db"));

var app = builder.Build();

// Automatyczne tworzenie bazy danych
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}


// ENDPOINTY

// 1. GET: Pobierz wszystkich użytkowników
app.MapGet("/users", async (AppDbContext db) =>
{
    var users = await db.Users.ToListAsync();
    return Results.Ok(users);
});

// 2. GET: Pobierz użytkownika po ID
app.MapGet("/users/{id}", async (int id, AppDbContext db) =>
{
    var user = await db.Users.FindAsync(id);
    return user is not null ? Results.Ok(user) : Results.NotFound();
});

// 3. POST: Dodaj nowego użytkownika
app.MapPost("/users", async (User user, AppDbContext db) =>
{
    if (!MiniValidator.TryValidate(user, out var errors))
        return Results.ValidationProblem(errors);

    db.Users.Add(user);
    await db.SaveChangesAsync();
    return Results.Created($"/users/{user.Id}", user);
});

// 4. PUT: Zaktualizuj użytkownika
app.MapPut("/users/{id}", async (int id, User updatedUser, AppDbContext db) =>
{
    var user = await db.Users.FindAsync(id);
    if (user is null) return Results.NotFound();

    if (!MiniValidator.TryValidate(updatedUser, out var errors))
        return Results.ValidationProblem(errors);

    user.Username = updatedUser.Username;
    user.Email = updatedUser.Email;
    await db.SaveChangesAsync();
    return Results.Ok(user);
});

// 5. DELETE: Usuń użytkownika
app.MapDelete("/users/{id}", async (int id, AppDbContext db) =>
{
    var user = await db.Users.FindAsync(id);
    if (user is null) return Results.NotFound();

    db.Users.Remove(user);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// 6. GET: Wyszukaj użytkownika po nazwie
app.MapGet("/users/search", async (string username, AppDbContext db) =>
{
    var users = await db.Users
        .Where(u => u.Username.Contains(username))
        .ToListAsync();

    return Results.Ok(users);
});

app.Run();


// MiniValidator: bardzo lekka walidacja (wbudowana tu dla 1 pliku)
static class MiniValidator
{
    public static bool TryValidate<T>(T obj, out Dictionary<string, string[]> errors)
    {
        var context = new ValidationContext(obj);
        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(obj, context, results, true);

        errors = results
            .GroupBy(r => r.MemberNames.FirstOrDefault() ?? "")
            .ToDictionary(
                g => g.Key,
                g => g.Select(r => r.ErrorMessage ?? "Invalid").ToArray()
            );

        return isValid;
    }
}
