using System.ComponentModel.DataAnnotations;

namespace ShopTex.Domain.Users;

public class UserEmail
{
    [EmailAddress]
    [CustomEmailAddress(Configs.ValidEmail)]
    public string Value { get; set; }

    private UserEmail() {}
    
    public UserEmail(string address)
    {
        Value = address;
    }
}