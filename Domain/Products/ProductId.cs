using Newtonsoft.Json;
using ShopTex.Domain.Shared;

namespace ShopTex.Domain.Products;

public class ProductId : EntityId
{
    [JsonConstructor]
    public ProductId(Guid value) : base(value)
    {
    }

    public ProductId(String value) : base(value)
    {
    }

    protected override Object createFromString(String text)
    {
        return new Guid(text);
    }

    public override String AsString()
    {
        Guid obj = (Guid)ObjValue;
        return obj.ToString();
    }

    public Guid AsGuid()
    {
        return (Guid)ObjValue;
    }
}