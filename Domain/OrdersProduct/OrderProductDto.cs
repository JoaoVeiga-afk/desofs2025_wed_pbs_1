using System.ComponentModel.DataAnnotations;
using ShopTex.Domain.Products;

namespace ShopTex.Domain.Orders;

public class OrderProductDto
{
    public ProductId ProductId { get; set; }

    public int Amount { get; set; }

    public double Price { get; set; }
}