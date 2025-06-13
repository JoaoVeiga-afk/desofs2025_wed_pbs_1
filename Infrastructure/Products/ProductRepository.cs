using Microsoft.EntityFrameworkCore;
using ShopTex.Domain.Products;
using ShopTex.Infrastructure.Shared;
using ShopTex.Models;

namespace ShopTex.Infrastructure.Products;

public class ProductRepository : BaseRepository<Product, ProductId>, IProductRepository
{
    public ProductRepository(DatabaseContext context) : base(context.Product)
    {

    }

    public async Task<Product?> FindById(string productId)
    {
        var id = new ProductId(productId); 
        return await _objs.FindAsync(id);
    }

    public Product Update(Product product)
    {

        return _objs.Update(product).Entity;

    }
}