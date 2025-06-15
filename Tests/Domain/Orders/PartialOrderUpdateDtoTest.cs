using ShopTex.Domain.Orders;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;
using FluentAssertions;

namespace ShopTex.Tests.Domain.Orders;

public class PartialOrderUpdateDtoTest
{
    [Theory]
    [InlineData("pending")]
    [InlineData("processing")]
    [InlineData("delivered")]
    [InlineData("cancelled")]
    public void ValidStatuses_ShouldPassValidation(string status)
    {
        var dto = new PartialOrderUpdateDto
        {
            Status = status
        };

        var results = new List<ValidationResult>();
        var context = new ValidationContext(dto);
        var isValid = Validator.TryValidateObject(dto, context, results, true);

        isValid.Should().BeTrue();
        results.Should().BeEmpty();
    }

    [Fact]
    public void InvalidStatus_ShouldFailValidation()
    {
        var dto = new PartialOrderUpdateDto
        {
            Status = "invalid-status"
        };

        var results = new List<ValidationResult>();
        var context = new ValidationContext(dto);
        var isValid = Validator.TryValidateObject(dto, context, results, true);

        isValid.Should().BeFalse();
        results.Should().ContainSingle(r => r.ErrorMessage!.Contains("Status must be one of"));
    }

    [Fact]
    public void NullStatus_ShouldPassValidation()
    {
        var dto = new PartialOrderUpdateDto
        {
            Status = null
        };

        var results = new List<ValidationResult>();
        var context = new ValidationContext(dto);
        var isValid = Validator.TryValidateObject(dto, context, results, true);

        isValid.Should().BeTrue(); // optional field
    }

    [Fact]
    public void Products_CanBeNull()
    {
        var dto = new PartialOrderUpdateDto
        {
            Status = "pending",
            Products = null
        };

        var results = new List<ValidationResult>();
        var context = new ValidationContext(dto);
        var isValid = Validator.TryValidateObject(dto, context, results, true);

        isValid.Should().BeTrue();
    }
}
