using System.ComponentModel.DataAnnotations;

public class RegisterUserRequest
{
    [Required]
    [MinLength(4)]
    public string Username { get; set; }

    [Required]
    [MinLength(6)]
    public string Password { get; set; }
}