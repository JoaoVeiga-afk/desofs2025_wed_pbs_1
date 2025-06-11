using ShopTex.Domain.Shared;

namespace ShopTex.Domain.Products;

public interface IProductRepository : IRepository<Product, ProductId>
{
    public Task<Product?> FindById(string ProductId);
    public Product Update(Product product);
}