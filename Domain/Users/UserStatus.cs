using ShopTex.Domain.Shared;

namespace ShopTex.Domain.Users;

/// <summary>
/// Stores the user's status (true is enabled and false is disabled)
/// </summary>
public class UserStatus
{
    public bool Value { get; set; }

    private UserStatus()
    {
    }

    public UserStatus(bool enabled)
    {
        Value = enabled;
    }

    public UserStatus(string status)
    {
        if (status == "enabled") Value = true;
        else if (status == "disabled") Value = false;
        else throw new BusinessRuleValidationException("Status can only be \"enabled\" or \"disabled\"");
    }

    public override string ToString()
    {
        return Value ? "enabled" : "disabled";
    }
}