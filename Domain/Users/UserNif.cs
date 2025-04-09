using System.ComponentModel.DataAnnotations;

namespace ShopTex.Domain.Users;

public class UserNif
{
    [Length(minimumLength:9, maximumLength:9)]
    public string? Value { get; set; }
    
    private UserNif() {}

    public UserNif(string nif)
    {
        Value = nif;
    }
}