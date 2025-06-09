// OrderRepository.cs
using Microsoft.EntityFrameworkCore;
using ShopTex.Domain.Orders;
using ShopTex.Infrastructure.Shared;
using ShopTex.Models;

namespace ShopTex.Infrastructure.Orders;

public class OrderRepository : BaseRepository<Order, OrderId>, IOrderRepository
{
    public OrderRepository(DatabaseContext context) : base(context.Order) { }

    public async Task<Order?> FindById(OrderId id)
    {
        return await _objs.FirstOrDefaultAsync(o => o.Id.Equals(id));
    }
    
    public Task<Order?> FindByIdWithProductsAsync(OrderId id)
        => _objs
            .Include(o => o.Products)           
            .FirstOrDefaultAsync(o => o.Id == id);

    
    public async Task<List<Order>> GetPagedAsync(int offset, int limit)
    {
        return await _objs
            .OrderBy(o => o.CreatedAt) 
            .Skip(offset)
            .Take(limit)
            .ToListAsync();
    }
    
    public Task DeleteAsync(Order order)
    {
        _objs.Remove(order);
        return Task.CompletedTask;
    }
}