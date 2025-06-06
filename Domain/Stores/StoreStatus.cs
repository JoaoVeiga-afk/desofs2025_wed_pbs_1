using ShopTex.Domain.Shared;

namespace ShopTex.Domain.Stores;

/// <summary>
/// Stores the store's status (true is enabled and false is disabled)
/// </summary>
public class StoreStatus
{
    public bool Value { get; set; }

    private StoreStatus()
    {
    }

    public StoreStatus(bool enabled)
    {
        Value = enabled;
    }

    public StoreStatus(string status)
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