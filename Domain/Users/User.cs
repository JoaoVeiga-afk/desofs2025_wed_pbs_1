using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using ShopTex.Domain.Shared;
using ShopTex.Domain.Stores;

namespace ShopTex.Domain.Users;

public class User : Entity<UserId>, IAggregateRoot
{

    public string Name { get; private set; }

    public string Phone { get; private set; }

    public UserEmail Email { get; private set; }

    private string Password { get; set; }

    private byte[] Salt { get; set; }

    public UserRole? Role { get; private set; }

    public UserStatus Status { get; private set; }

    public StoreId? Store { get; private set; }

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
        SetStatusAutomatically();
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

    public bool VerifyPassword(string password)
    {

        var hashed = this.Password;

        return this.Password.Equals(Configurations.HashString(password, this.Salt));
    }

    public bool EnableUser()
    {
        if (Status == null || Status.ToString().Equals("disabled", StringComparison.OrdinalIgnoreCase) == true)
        {
            Status = new UserStatus("enabled");
            return true;
        }
        return false;
    }

    public bool DisableUser()
    {
        if (Status == null || Status.ToString().Equals("enabled", StringComparison.OrdinalIgnoreCase) == true)
        {
            Status = new UserStatus("disabled");
            return true;
        }
        return false;
    }

    public bool SetStore(string storeId)
    {
        if (Role == null) return false;

        if (Role.RoleName == Configurations.STORE_COLAB_ROLE_NAME || Role.RoleName == Configurations.STORE_ADMIN_ROLE_NAME)
        {
            Store = new StoreId(storeId);
            return true;
        }

        return false;
    }


    public void ChangeRole(UserRole role)
    {
        if (role.RoleName != Configurations.STORE_COLAB_ROLE_NAME && role.RoleName != Configurations.STORE_ADMIN_ROLE_NAME)
        {
            Store = null;
        }
        Role = role;
        SetStatusAutomatically();
    }

    private void SetStatusAutomatically()
    {
        if (Role == null)
        {
            DisableUser();
            return;
        }

        switch (Role.RoleName)
            {
                case Configurations.SYS_ADMIN_ROLE_NAME:
                case Configurations.STORE_ADMIN_ROLE_NAME:
                    DisableUser();
                    break;
                default:
                    EnableUser();
                    break;
            }
    }
}
