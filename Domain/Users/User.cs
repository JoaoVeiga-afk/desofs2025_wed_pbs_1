using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using ShopTex.Domain.Shared;

namespace ShopTex.Domain.Users;

public class User : Entity<UserId>, IAggregateRoot
{

    public string Name { get; set; }

    public string Phone { get; set; }

    public UserEmail Email { get; set; }

    public string Password { get; set; }

    public byte[] Salt { get; set; }

    public UserRole? Role { get; set; }

    public UserStatus Status { get; set; }

    private User()
    {
    }

    public User(string name, string phone, string email, string password, string? role, string status, byte[] salt)
    {
        Id = new UserId(Guid.NewGuid());
        Name = name;
        Phone = phone;
        Email = new UserEmail(email);
        Password = password;
        Role = role != null ? UserRole.SelectRoleFromString(role) : null;
        Salt = salt;
        Status = new UserStatus(status);
    }
    public User(string id, string name, string phone, string email, string password, string? role, string status, byte[] salt)
    {
        Id = new UserId(id);
        Name = name;
        Phone = phone;
        Email = new UserEmail(email);
        Password = password;
        Role = role != null ? UserRole.SelectRoleFromString(role) : null;
        Salt = salt;
        Status = new UserStatus(status);
    }
}
