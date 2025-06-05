using Newtonsoft.Json;
using ShopTex.Domain.Shared;

namespace ShopTex.Domain.Stores;

public class StoreId : EntityId
{
    [JsonConstructor]
    public StoreId(Guid value) : base(value)
    {
    }

    public StoreId(String value) : base(value)
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