// CreatingOrderDto.cs
using System.ComponentModel.DataAnnotations;
using ShopTex.Domain.Users;

namespace ShopTex.Domain.Orders;

public class CreatingOrderDto
{
    [Required]
    public Guid UserId { get; set; }
    
    [Required]
    public string Status { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "At least one product must be included in the order.")]
    public List<CreatingOrderProductDto> Products { get; set; } = new();
}