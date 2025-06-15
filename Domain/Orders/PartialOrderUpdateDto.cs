using System.ComponentModel.DataAnnotations;

namespace ShopTex.Domain.Orders;

public class PartialOrderUpdateDto
{
    [RegularExpression("pending|processing|delivered|cancelled",
        ErrorMessage = "Status must be one of: pending, processing, delivered, cancelled")]
    public string? Status { get; set; }

    public List<CreatingOrderProductDto>? Products { get; set; }
}