using System;
using FluentAssertions;
using ShopTex.Domain.Shared.Validation;
using Xunit;

namespace ShopTex.Tests.Domain.Shared.Validation
{
    public class PasswordValidationAttributeTest
    {
        private readonly PasswordValidationAttribute _attribute = new();

        [Fact]
        public void IsValid_WithValidPassword_ShouldReturnTrue()
        {
            var validPassword = "StrongP@ssw0rd2025!";

            var result = _attribute.IsValid(validPassword);

            result.Should().BeTrue();
        }

        [Fact]
        public void IsInvalid_WithMissingUppercase_ShouldReturnFalse()
        {
            var password = "weakp@ssword2025!";

            var result = _attribute.IsValid(password);

            result.Should().BeFalse();
        }

        [Fact]
        public void IsInvalid_WithMissingLowercase_ShouldReturnFalse()
        {
            var password = "STRONG@2025PASSWORD!";

            var result = _attribute.IsValid(password);

            result.Should().BeFalse();
        }

        [Fact]
        public void IsInvalid_WithMissingDigit_ShouldReturnFalse()
        {
            var password = "StrongPassword@!";

            var result = _attribute.IsValid(password);

            result.Should().BeFalse();
        }

        [Fact]
        public void IsInvalid_WithMissingSpecialCharacter_ShouldReturnFalse()
        {
            var password = "StrongPassword2025";

            var result = _attribute.IsValid(password);

            result.Should().BeFalse();
        }

        [Fact]
        public void IsInvalid_WithLessThan16Characters_ShouldReturnFalse()
        {
            var password = "Sh0rt@Pwd!";

            var result = _attribute.IsValid(password);

            result.Should().BeFalse();
        }

        [Fact]
        public void IsInvalid_WithNullPassword_ShouldReturnFalse()
        {
            var result = _attribute.IsValid(null);

            result.Should().BeFalse();
        }

        [Fact]
        public void IsInvalid_WithEmptyPassword_ShouldReturnFalse()
        {
            var result = _attribute.IsValid("");

            result.Should().BeFalse();
        }
    }
}
