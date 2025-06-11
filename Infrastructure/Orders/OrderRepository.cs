// OrderRepository.cs
using Microsoft.EntityFrameworkCore;
using ShopTex.Domain.Orders;
using ShopTex.Infrastructure.Shared;
using ShopTex.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

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
            // Passa o VO OrderId; o EF usa seu converter para extrair o Guid
            var order = await _objs.FindAsync(id);
            return order;
        }
        
        public async Task<Order?> FindByIdWithProductsAsync(OrderId id)
        {
            // 1) FindAsync com o VO
            var order = await _objs.FindAsync(id);
            if (order == null) return null;

            // 2) depois carrega a coleção de produtos
            await _context.Entry(order)
                .Collection(o => o.Products)
                .LoadAsync();

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
        
        public Task DeleteAsync(Order order)
        {
            _objs.Remove(order);
            return Task.CompletedTask;
        }
    }
}