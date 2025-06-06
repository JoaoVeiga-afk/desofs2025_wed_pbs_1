namespace ShopTex.Domain.Stores;

public class StoreDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Address { get; set; }

    public string Status { get; set; }

    public StoreDto(Guid id, string name, StoreAddress address, StoreStatus status)
    {
        Id = id;
        Name = name;
        Address = address.ToString();
        Status = status.ToString();
    }
}