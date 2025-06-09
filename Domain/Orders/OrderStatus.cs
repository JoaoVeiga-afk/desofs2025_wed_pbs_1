using ShopTex.Domain.Shared;

namespace ShopTex.Domain.Orders;

public class OrderStatus
{
    public string Value { get; private set; }

    private static readonly HashSet<string> Allowed = new()
    {
        "pending", "processing", "delivered", "cancelled"
    };

    private OrderStatus() { }

    public OrderStatus(string status)
    {
        if (!Allowed.Contains(status.ToLower()))
            throw new BusinessRuleValidationException("Invalid order status. Allowed: pending, processing, delivered, cancelled");

        Value = status.ToLower();
    }

    public override string ToString() => Value;
}