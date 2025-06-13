using ShopTex.Domain.Shared;

namespace ShopTex.Domain.Users;

public class UserRole
{
    public string RoleName { get; private set; }
    public string Description { get; private set; }

    private UserRole()
    {

    }

    private UserRole(string name, string description)
    {
        RoleName = name;
        Description = description;
    }


    public static readonly UserRole SystemRole = new UserRole(Configurations.SYS_ADMIN_ROLE_NAME, "Has full access to the system, including user management and configuration settings.");
    public static readonly UserRole UserNRole = new UserRole(Configurations.USER_ROLE_NAME, "A regular");
    public static readonly UserRole StoreAdminRole = new UserRole(Configurations.STORE_ADMIN_ROLE_NAME, "Has access to manage store-specific settings and operations.");
    public static readonly UserRole StoreColabRole = new UserRole(Configurations.STORE_COLAB_ROLE_NAME, "Can manage store operations but has limited access to user management.");


    public static UserRole SelectRoleFromString(string? val)
    {
        if (val is { Length: 0 }) return null;
        switch (val.ToLower())
        {
            case "system":
            case "system administrator":
                return SystemRole;

            case "client":
                return UserNRole;

            case "store administrator":
                return StoreAdminRole;

            case "store collaborator":
                return StoreColabRole;

            default:
                throw new BusinessRuleValidationException("Role field is Invalid");
        }
    }

    public override bool Equals(object? obj)
    {
        if (obj is UserRole other)
        {
            return RoleName.Equals(other.RoleName, StringComparison.OrdinalIgnoreCase);
        }
        return false;
    }
}
