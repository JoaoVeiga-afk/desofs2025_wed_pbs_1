using ShopTex.Domain.Orders;
using ShopTex.Domain.Products;

namespace ShopTex.Domain.OrdersProduct
{
    public class OrderProduct
    {
        public OrderId OrderId { get; private set; }
        public ProductId ProductId { get; private set; }
        public int Amount { get; private set; }
        public double Price { get; private set; }

        public Order Order { get; private set; } = null!;
        public Product Product { get; private set; } = null!; // <- NOVO

        private OrderProduct() { }

        public OrderProduct(OrderId orderId, ProductId productId, int amount, double price)
        {
            OrderId = orderId;
            ProductId = productId;
            Amount = amount;
            Price = price;
        }
    }
}