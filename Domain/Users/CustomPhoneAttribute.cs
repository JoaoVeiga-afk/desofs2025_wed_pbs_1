using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace UserManager.Domain.Users;

public class CustomPhoneAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value == null)
        {
            return true;
        }
        
        string phoneNumber = value.ToString();
        
        // remove all empty spaces
        phoneNumber = Regex.Replace(phoneNumber, @"\s", "");

        if (IsValidPhoneNumber(phoneNumber)) return true;

        ErrorMessage = $"{phoneNumber} is not a valid Portuguese phone number";

        return false;
    }

    private bool IsValidPhoneNumber(string phonenumber)
    {
        string regex = @"^(\+351)?(9\d{8}|[2356]\d{8})$";
        return Regex.IsMatch(phonenumber, regex);
    }
}