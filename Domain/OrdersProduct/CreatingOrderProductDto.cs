using System.ComponentModel.DataAnnotations;
using ShopTex.Domain.Products;

namespace ShopTex.Domain.Orders;

public class CreatingOrderProductDto
{
    [Required]
    public Guid? ProductId { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Amount must be at least 1.")]
    public int Amount { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
    public double Price { get; set; }
}