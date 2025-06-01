using System.ComponentModel.DataAnnotations;

namespace ShopTex.Domain.Users;

public class CreatingUserDto
{
    [Required]
    public string Name { get; set; }

    [Required]
    [CustomPhone]
    public string Phone { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }

    public string? RoleId { get; set; }
    
}
