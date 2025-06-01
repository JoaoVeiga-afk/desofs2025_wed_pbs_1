using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using ShopTex.Domain.Shared;

namespace ShopTex.Domain.Users;

public class User : Entity<UserId>, IAggregateRoot
{

    public  string Name { get; private set; }

    public  string Phone { get; private set; }

    public  UserEmail Email { get; private set; }

    private  string Password { get; set; }

    private  byte[] Salt { get; set; }

    public  UserRole? Role { get; private set; }

    public  UserStatus Status { get; private set; }

    private User()
    {
    }

    public User(string name, string phone, string email, string password, string? role, byte[] salt)
    {
        Id = new UserId(Guid.NewGuid());
        Name = name;
        Phone = phone;
        Email = new UserEmail(email);
        Password = password;
        Role = role != null ? UserRole.SelectRoleFromString(role) : null;
        Salt = salt;
        if (role != null)
        {
            switch (Role.RoleName) 
            {
                case Configurations.SYS_ADMIN_ROLE_NAME:
                case Configurations.STORE_ADMIN_ROLE_NAME:
                    Status = new UserStatus("disabled");
                    break;
                default:
                    Status = new UserStatus("enabled");
                    break;
            }
        }
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
    
    public bool VerifyPassword(string password){
    
        var hashed= this.Password;
        
        return this.Password.Equals(Configurations.HashString(password, this.Salt));
    }
}
