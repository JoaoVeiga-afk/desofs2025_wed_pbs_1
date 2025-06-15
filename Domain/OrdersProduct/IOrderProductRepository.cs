using ShopTex.Domain.Orders;
using ShopTex.Domain.OrdersProduct;

namespace ShopTex.Domain.OrdersProduct
{
    public interface IOrderProductRepository
    {
        Task AddAsync(OrderProduct entity);
        Task<List<OrderProduct>> GetByOrderIdAsync(OrderId orderId);
        Task DeleteByOrderIdAsync(OrderId orderId);
    }


}