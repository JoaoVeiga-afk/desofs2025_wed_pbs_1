using System.ComponentModel.DataAnnotations;

namespace ShopTex.Domain.Users;

public class CreatingUserDto
{
    public string? Id { get; set; }
    
    [Required]
    public string FirstName { get; set; }
    
    [Required]
    public string LastName { get; set; }
    
    [Required]
    [CustomPhone]
    public string Phone { get; set; }
    
    [Required]
    [EmailAddress]
    [CustomEmailAddress(Configs.ValidEmail)]
    public string Email { get; set; }
    
    [Required]
    public string Password { get; set; }
    
    public string? Role { get; set; }
    
    public string? Nif { get; set; }
    
    [Required]
    [RegularExpression("^(enabled)$|^(disabled)$")]
    public string Status { get; set; }
}