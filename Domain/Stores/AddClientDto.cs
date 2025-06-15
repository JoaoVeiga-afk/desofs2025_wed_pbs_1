namespace ShopTex.Domain.Stores;

public class AddClientDto
{
    public Guid StoreId { get; set; }
    public Guid? UserId { get; set; }
}
