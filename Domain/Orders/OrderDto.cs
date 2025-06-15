using System.ComponentModel.DataAnnotations;

namespace ShopTex.Domain.Orders;

public class OrderDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<OrderProductDto> Products { get; set; } = new();
}