using ShopTex.Domain.Shared;

namespace ShopTex.Domain.Products;

public class ProductStatus
{
    public bool Value { get; set; }

    private ProductStatus()
    {
    }

    public ProductStatus(bool enabled)
    {
        Value = enabled;
    }

    public ProductStatus(string status)
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