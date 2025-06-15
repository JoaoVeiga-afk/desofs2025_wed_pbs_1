using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ShopTex.Domain.Shared.Validation
{
    public class PasswordValidationAttribute : ValidationAttribute
    {
        private const string DefaultErrorMessage = "The password must contain at least 16 characters, including uppercase and lowercase letters, numbers, and special characters.";

        public PasswordValidationAttribute() : base(DefaultErrorMessage) { }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult(ErrorMessage ?? DefaultErrorMessage);
            }

            var password = value.ToString();

            var hasUpperCase = new Regex(@"[A-Z]").IsMatch(password);
            var hasLowerCase = new Regex(@"[a-z]").IsMatch(password);
            var hasDigits = new Regex(@"[0-9]").IsMatch(password);
            var hasSpecialChar = new Regex(@"[\W_]").IsMatch(password);
            var isLongEnough = password.Length >= 16;

            if (hasUpperCase && hasLowerCase && hasDigits && hasSpecialChar && isLongEnough)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(ErrorMessage ?? DefaultErrorMessage);
        }

    }
}