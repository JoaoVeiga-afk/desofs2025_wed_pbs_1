using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using ShopTex.Domain.Shared;

namespace ShopTex.Domain.Stores;

public class Store : Entity<StoreId>, IAggregateRoot
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [Required]
    public StoreAddress Address { get; set; }

    [Required]
    public StoreStatus Status { get; set; }

    private Store()
    {
    }

    public Store(string name, StoreAddress address, string status)
    {
        Id = new StoreId(Guid.NewGuid());
        Name = name;
        Address = address;
        Status = new StoreStatus(status);

        Validate();
    }

    public Store(string id, string name, StoreAddress address, string status)
    {
        Id = new StoreId(id);
        Name = name;
        Address = address;
        Status = new StoreStatus(status);

        Validate();
    }

    private void Validate()
    {
        Validator.ValidateObject(this, new ValidationContext(this), validateAllProperties: true);
    }
}
