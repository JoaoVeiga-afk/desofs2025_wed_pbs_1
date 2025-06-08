using System.ComponentModel.DataAnnotations;
using ShopTex.Domain.Shared;
using ShopTex.Domain.Stores;

namespace ShopTex.Domain.Products;

public class Product : Entity<ProductId>, IAggregateRoot
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [StringLength(250)]
    public string Description { get; set; }

    [Required]
    public double Price { get; set; }

    public string Category { get; set; }

    [Required]
    public ProductStatus Status { get; set; }

    [Required]
    public StoreId StoreId { get; set; }

    public Product(string name, string? description, double price, string? category, string status, string storeId)
    {
        Id = new ProductId(Guid.NewGuid());
        Name = name;
        Description = description ?? "";
        Price = price;
        Category = category ?? "";
        StoreId = new StoreId(storeId);
        Status = new ProductStatus(status);

        Validate();
    }

    public Product(string id, string name, string description, float price, string category, string status, string storeId)
    {
        Id = new ProductId(id);
        Name = name;
        Description = description;
        Price = price;
        Category = category;
        StoreId = new StoreId(storeId);
        Status = new ProductStatus(status);

        Validate();
    }

    private void Validate()
    {
        Validator.ValidateObject(this, new ValidationContext(this), validateAllProperties: true);
    }
}
