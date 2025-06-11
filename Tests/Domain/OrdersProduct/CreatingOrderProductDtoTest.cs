using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using ShopTex.Domain.Orders;
using Xunit;

namespace ShopTex.Tests.Domain.OrdersProduct;

public class CreatingOrderProductDtoTest
{
    [Fact]
    public void ValidDto_ShouldPassValidation()
    {
        var dto = new CreatingOrderProductDto
        {
            ProductId = Guid.NewGuid(),
            Amount = 2,
            Price = 10.5
        };

        var results = new List<ValidationResult>();
        var context = new ValidationContext(dto);
        var isValid = Validator.TryValidateObject(dto, context, results, true);

        isValid.Should().BeTrue();
        results.Should().BeEmpty();
    }

    [Fact]
    public void AmountLessThanOne_ShouldFailValidation()
    {
        var dto = new CreatingOrderProductDto
        {
            ProductId = Guid.NewGuid(),
            Amount = 0,
            Price = 10.5
        };

        var results = new List<ValidationResult>();
        var context = new ValidationContext(dto);
        var isValid = Validator.TryValidateObject(dto, context, results, true);

        isValid.Should().BeFalse();
        results.Should().Contain(r => r.ErrorMessage.Contains("Amount must be at least 1"));
    }

    [Fact]
    public void PriceLessThanOrEqualToZero_ShouldFailValidation()
    {
        var dto = new CreatingOrderProductDto
        {
            ProductId = Guid.NewGuid(),
            Amount = 2,
            Price = 0
        };

        var results = new List<ValidationResult>();
        var context = new ValidationContext(dto);
        var isValid = Validator.TryValidateObject(dto, context, results, true);

        isValid.Should().BeFalse();
        results.Should().Contain(r => r.ErrorMessage.Contains("Price must be greater than zero"));
    }
}