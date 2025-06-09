using ShopTex.Domain.OrdersProduct;
using ShopTex.Domain.Shared;
using ShopTex.Domain.Users;

namespace ShopTex.Domain.Orders;

public class Order : Entity<OrderId>, IAggregateRoot
{
    public UserId UserId { get; private set; } 
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private readonly List<OrderProduct> _products = new();
    public IReadOnlyCollection<OrderProduct> Products => _products.AsReadOnly();

    private Order() { }

    public Order(UserId  userId, string status)
    {
        Id        = new OrderId(Guid.NewGuid());
        UserId    = userId;
        Status    = new OrderStatus(status);
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddProduct(int productId, int amount, double price)
    {
        var orderProduct = new OrderProduct(this.Id, productId, amount, price);
        _products.Add(orderProduct);

        UpdatedAt = DateTime.UtcNow;
    }

    public void SetStatus(string newStatus)
    {
        Status    = new OrderStatus(newStatus);
        UpdatedAt = DateTime.UtcNow;
    }
}