using ShopTex.Domain.Shared;

namespace ShopTex.Domain.Users;

public class UserRole
{
    public string RoleName { get; private set; }

    private UserRole()
    {
        
    }

    private UserRole(string name)
    {
        RoleName = name;
    }

    public static readonly UserRole CampusRole = new UserRole("Campus Manager");
    public static readonly UserRole FleetRole = new UserRole("Fleet Manager");
    public static readonly UserRole TaskRole = new UserRole("Task Manager");
    public static readonly UserRole SystemRole = new UserRole("System Administrator");
    public static readonly UserRole UserNRole = new UserRole("User");

    public static UserRole SelectRoleFromString(string? val)
    {
        if (val is { Length: 0 }) return null;
        switch (val.ToLower())
        {
            case "campus": 
            case "campus manager":
                return CampusRole;
            
            case "fleet":
            case "fleet manager":
                return FleetRole;
            
            case "task":
            case "task manager":
                return TaskRole;
            
            case "system":
            case "system administrator":
                return SystemRole;
            
            case "user":
                return UserNRole;
            
            default:
                throw new BusinessRuleValidationException("Role field is Invalid");
        }
    }
}