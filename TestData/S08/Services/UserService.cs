namespace MusicApp.Services;

public class UserService
{
    public bool Authenticate(string user, string pass)
    {
        return user == "admin" && pass == "pass123";
    }
}