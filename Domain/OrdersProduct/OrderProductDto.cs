using System.ComponentModel.DataAnnotations;

namespace ShopTex.Domain.Orders;

public class OrderProductDto
{
    public int ProductId { get; set; }

    public int Amount { get; set; }

    public double Price { get; set; }
}