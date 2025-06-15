using ShopTex.Domain.Stores;

namespace ShopTex.Domain.Products;

public class ProductDto
{
    public string Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public double Price { get; set; }

    public string Category { get; set; }

    public string Status { get; set; }

    public string StoreId { get; set; }

    public string? ImageUrl { get; set; }

    public ProductDto(string id, string name, string description, double price, string category, ProductStatus status, StoreId storeId, string? imageUrl = null)
    {
        Id = id;
        Name = name;
        Price = price;
        Category = category;
        Description = description;
        Status = status.ToString();
        StoreId = storeId.AsString();
        ImageUrl = imageUrl;
    }
}