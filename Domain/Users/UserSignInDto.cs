using System.ComponentModel.DataAnnotations;

namespace UserManager.Domain.Users;

public class UserSignInDto
{
    [Required]
    [EmailAddress]
    [CustomEmailAddress(Configs.ValidEmail)]
    public string Email { get; set; } 
    
    [Required]
    public string Password { get; set; }
}