// OrderRepository.cs
using Microsoft.EntityFrameworkCore;
using ShopTex.Domain.Orders;
using ShopTex.Infrastructure.Shared;
using ShopTex.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using ShopTex.Domain.Stores;
using ShopTex.Domain.Users;

namespace ShopTex.Infrastructure.Orders
{
    public class OrderRepository : BaseRepository<Order, OrderId>, IOrderRepository
    {
        private readonly DatabaseContext _context;

        public OrderRepository(DatabaseContext context) 
            : base(context.Order)
        {
            _context = context;
        }
        
        public async Task<Order?> FindById(OrderId id)
        {
            var order = await _objs.FindAsync(id);
            return order;
        }
        
        public async Task<Order?> FindByIdWithProductsAsync(OrderId id)
        {
            var order = await _objs.FindAsync(id);
            if (order == null) return null;

            await _context.Entry(order)
                .Collection(o => o.Products)
                .LoadAsync();

            return order;
        }

        public async Task<Order?> FindByIdByUserAsync(UserId userId, OrderId orderId)
        {
            var order = await _objs
                .Where(o => o.UserId == userId && o.Id == orderId)
                .FirstOrDefaultAsync();
            if (order != null)
                await _context.Entry(order).Collection(o => o.Products).LoadAsync();
            return order;
        }

        public async Task<Order?> FindByIdByStoreAsync(StoreId storeVo, OrderId orderId)
        {
            var order = await _context.Order
                .Where(o => o.Id == orderId)
                .Where(o => _context.User
                    .Any(u => u.Id == o.UserId && u.Store == storeVo))
                .FirstOrDefaultAsync();
            if (order != null)
                await _context.Entry(order).Collection(o => o.Products).LoadAsync();
            return order;
        }

        public async Task<List<Order>> GetPagedAsync(int offset, int limit)
        {
            return await _objs
                .OrderBy(o => o.CreatedAt)
                .Skip(offset)
                .Take(limit)
                .ToListAsync();
        }
        
        public async Task<List<Order>> GetPagedByStoreAsync(Guid storeId, int offset, int limit)
        { 
            var storeVo = new StoreId(storeId.ToString());

            return await _context.Order
                .Where(o => _context.User
                    .Any(u =>
                            u.Id     == o.UserId &&
                            u.Store  == storeVo     
                    )
                )
                .OrderBy(o => o.CreatedAt)
                .Skip(offset)
                .Take(limit)
                .ToListAsync();
        }
        
        public async Task<List<Order>> GetPagedByUserAsync(Guid userId, int offset, int limit)
        {
            return await _objs
                .Where(o => o.UserId == new UserId(userId))
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
}
