using System.Collections.Concurrent;

public class UserAccountService
{
    private readonly ConcurrentDictionary<string, string> _users = new();

    public bool Register(RegisterUserRequest request)
    {
        return _users.TryAdd(request.Username, request.Password);
    }

    public bool Login(LoginRequest request)
    {
        return _users.TryGetValue(request.Username, out var storedPassword) && storedPassword == request.Password;
    }
}