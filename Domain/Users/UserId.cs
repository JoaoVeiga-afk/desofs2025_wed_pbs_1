using Newtonsoft.Json;
using ShopTex.Domain.Shared;

namespace ShopTex.Domain.Users;

public class UserId : EntityId
{
    [JsonConstructor]
    public UserId(Guid value) : base(value)
    {
    }

    public UserId(String value) : base(value)
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