namespace ShopTex.Domain.Users;

public class UserDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Phone { get; set; }

    public string Email { get; set; }

    public string? Role { get; set; }

    public string Status { get; set; }

    public UserDto(Guid id, string name, string phone, UserEmail email, UserRole role, UserStatus status)
    {
        Id = id;
        Name = name;
        Phone = phone;
        Email = email.Value;
        Role =  role != null ? role.RoleName : null;
        Status = status.ToString();
    }
}
