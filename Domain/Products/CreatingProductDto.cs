using ShopTex.Domain.Stores;

namespace ShopTex.Domain.Products;

public class CreatingProductDto
{
    public string Name { get; set; }

    public string Description { get; set; }

    public double Price { get; set; }

    public string Category { get; set; }

    public string Status { get; set; }

    public string StoreId { get; set; }

    public CreatingProductDto() { }

    public CreatingProductDto(string name, string description, double price, string category, ProductStatus status, StoreId storeId)
    {
        Name = name;
        Price = price;
        Category = category;
        Description = description;
        Status = status.ToString();
        StoreId = storeId.AsString();
    }
}