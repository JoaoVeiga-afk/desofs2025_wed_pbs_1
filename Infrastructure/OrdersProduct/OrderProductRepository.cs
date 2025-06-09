using Microsoft.EntityFrameworkCore;
using ShopTex.Domain.OrdersProduct;
using ShopTex.Domain.Orders;
using ShopTex.Models;

namespace ShopTex.Infrastructure.OrdersProduct;

public class OrderProductRepository : IOrderProductRepository
{
    private readonly DatabaseContext _context;
    public OrderProductRepository(DatabaseContext context)
        => _context = context;

    public async Task AddAsync(OrderProduct entity)
    {
        _context.OrderProduct.Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<List<OrderProduct>> GetByOrderIdAsync(OrderId orderId)
    {
        // Aqui comparamos VO ↔ VO, sem tocar em Guid  
        return await _context.OrderProduct
            .Where(op => op.OrderId == orderId)
            .ToListAsync();
    }

    public async Task DeleteByOrderIdAsync(OrderId orderId)
    {
        var items = await _context.OrderProduct
            .Where(op => op.OrderId == orderId)
            .ToListAsync();

        _context.OrderProduct.RemoveRange(items);
        await _context.SaveChangesAsync();
    }
}