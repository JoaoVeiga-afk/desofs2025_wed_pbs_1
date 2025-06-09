// ShopTex.Domain.OrdersProduct.OrderProduct.cs
using ShopTex.Domain.Orders;

namespace ShopTex.Domain.OrdersProduct
{
    public class OrderProduct
    {
        public OrderId OrderId   { get; private set; }
        public int     ProductId { get; private set; }
        public int     Amount    { get; private set; }
        public double  Price     { get; private set; }

        public Order  Order      { get; private set; }

        private OrderProduct() { }

        public OrderProduct(OrderId orderId, int productId, int amount, double price)
        {
            OrderId   = orderId;
            ProductId = productId;
            Amount    = amount;
            Price     = price;
        }
    }
}