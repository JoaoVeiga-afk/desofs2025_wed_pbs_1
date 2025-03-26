using System.ComponentModel.DataAnnotations;

namespace UserManager.Domain.Users;

public class CustomEmailAddressAttribute : ValidationAttribute
{
    private readonly string allowedDomain;

    public CustomEmailAddressAttribute(string allowedDomain)
    {
        this.allowedDomain = allowedDomain;
    }

    public override bool IsValid(object value)
    {
        if (value == null)
        {
            return true;
        }

        string email = value.ToString();
        string[] parts = email.Split("@");

        if (parts.Length == 2 && parts[1].ToLower() == allowedDomain.ToLower())
        {
            return true;
        }

        ErrorMessage = $"Email must be from the {allowedDomain} domain!";
        return false;
    }
}