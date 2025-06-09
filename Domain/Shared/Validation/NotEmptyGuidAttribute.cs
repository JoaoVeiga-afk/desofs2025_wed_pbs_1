using System;
using System.ComponentModel.DataAnnotations;

namespace ShopTex.Domain.Shared.Validation
{
    public class NotEmptyGuidAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is Guid guidValue)
                return guidValue != Guid.Empty;

            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} cannot be empty.";
        }
    }
}