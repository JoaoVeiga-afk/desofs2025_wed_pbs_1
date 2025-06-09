using ShopTex.Domain.Shared;

namespace ShopTex.Domain.Orders;

public interface IOrderRepository : IRepository<Order, OrderId>
{
    Task<Order?> FindById(OrderId id);
    Task<Order?> FindByIdWithProductsAsync(OrderId id);

    Task<List<Order>> GetPagedAsync(int offset, int limit);
    
    Task DeleteAsync(Order order);

}