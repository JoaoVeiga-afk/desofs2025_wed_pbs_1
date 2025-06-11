using Newtonsoft.Json;
using ShopTex.Domain.Shared;

namespace ShopTex.Domain.Orders;

public class OrderId : EntityId
{
    [JsonConstructor]
    public OrderId(Guid value) : base(value) { }

    public OrderId(string value) : base(new Guid(value)) { }

    protected override object createFromString(string text)
    {
        return new Guid(text);
    }

    public override string AsString()
    {
        return ((Guid)ObjValue).ToString();
    }

    public Guid AsGuid()
    {
        return (Guid)ObjValue;
    }
}