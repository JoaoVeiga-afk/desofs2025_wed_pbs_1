using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using UserManager.Domain.Shared;

namespace UserManager.Domain.Users;

public class User : Entity<UserId>, IAggregateRoot
{
    
    public string FirstName { get; set; }
    
    public string LastName { get; set; }
    
    public string Phone { get; set; }
    
    public UserEmail Email { get; set; }
    
    public string Password { get; set; }
    
    public byte[] Salt { get; set; }
    
    public UserRole? Role { get; set; }
    
    public UserNif? Nif { get; set; }
    public UserStatus Status { get; set; }

    private User()
    {
    }

    public User(string firstName, string lastName, string phone, string email, string password, string? role, string nif, string status, byte[] salt)
    {
        Id = new UserId(Guid.NewGuid());
        FirstName = firstName;
        LastName = lastName;
        Phone = phone;
        Email = new UserEmail(email);
        Password = password;
        Role = role != null ? UserRole.SelectRoleFromString(role) : null;
        Salt = salt;
        Nif = new UserNif(nif);
        Status = new UserStatus(status);
    }
    public User(string id, string firstName, string lastName, string phone, string email, string password, string? role, string nif, string status, byte[] salt)
    {
        Id = new UserId(id);
        FirstName = firstName;
        LastName = lastName;
        Phone = phone;
        Email = new UserEmail(email);
        Password = password;
        Role = role != null ? UserRole.SelectRoleFromString(role) : null;
        Salt = salt;
        Nif = new UserNif(nif);
        Status = new UserStatus(status);
    }
}