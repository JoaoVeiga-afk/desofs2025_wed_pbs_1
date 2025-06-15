using ShopTex.Domain.Shared;
using ShopTex.Domain.Stores;
using ShopTex.Domain.Users;

namespace ShopTex.Domain.Orders;

public interface IOrderRepository : IRepository<Order, OrderId>
{
    Task<Order?> FindById(OrderId id);
    Task<Order?> FindByIdWithProductsAsync(OrderId id);
    
    Task<Order?> FindByIdByUserAsync(UserId userId, OrderId orderId);
    Task<Order?> FindByIdByStoreAsync(StoreId storeId, OrderId orderId);

    Task<List<Order>> GetPagedAsync(int offset, int limit);
    
    Task<List<Order>> GetPagedByStoreAsync(Guid storeId, int offset, int limit);

    Task<List<Order>> GetPagedByUserAsync(Guid userId, int offset, int limit);

    Task DeleteAsync(Order order);

}