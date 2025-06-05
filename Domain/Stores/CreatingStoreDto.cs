using System.ComponentModel.DataAnnotations;

namespace ShopTex.Domain.Stores;

public class CreatingStoreDto
{
    [Required]
    public string Name { get; set; }

    [Required]
    public StoreAddress Address { get; set; }

    [Required]
    [RegularExpression("^(enabled)$|^(disabled)$", ErrorMessage = "Status must be either 'enabled' or 'disabled'.")]
    public string Status { get; set; }
}