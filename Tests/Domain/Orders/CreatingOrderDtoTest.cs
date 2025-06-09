using ShopTex.Domain.Orders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;
using FluentAssertions;

namespace ShopTex.Tests.Domain.Orders;

public class CreatingOrderDtoTest
{
    [Fact]
    public void ValidDto_ShouldPassValidation()
    {
        var dto = new CreatingOrderDto
        {
            UserId = Guid.NewGuid(),
            Status = "pending",
            Products = new List<CreatingOrderProductDto>
            {
                new() { ProductId = 1, Amount = 1, Price = 9.99 }
            }
        };

        var results = new List<ValidationResult>();
        var context = new ValidationContext(dto);
        var isValid = Validator.TryValidateObject(dto, context, results, true);

        isValid.Should().BeTrue();
        results.Should().BeEmpty();
    }

    [Fact]
    public void MissingUserId_ShouldFailValidation()
    {
        var dto = new CreatingOrderDto
        {
            Status = "pending",
            Products = new List<CreatingOrderProductDto>
            {
                new() { ProductId = 1, Amount = 1, Price = 9.99 }
            }
        };

        var results = new List<ValidationResult>();
        var context = new ValidationContext(dto);
        var isValid = Validator.TryValidateObject(dto, context, results, true);

        isValid.Should().BeFalse();
        results.Should().Contain(r => r.MemberNames.Contains(nameof(CreatingOrderDto.UserId)));
    }

    [Fact]
    public void EmptyProducts_ShouldFailValidation()
    {
        var dto = new CreatingOrderDto
        {
            UserId = Guid.NewGuid(),
            Status = "pending",
            Products = new List<CreatingOrderProductDto>() // empty list
        };

        var results = new List<ValidationResult>();
        var context = new ValidationContext(dto);
        var isValid = Validator.TryValidateObject(dto, context, results, true);

        isValid.Should().BeFalse();
        results.Should().Contain(r => r.ErrorMessage!.Contains("At least one product"));
    }

    [Fact]
    public void MissingStatus_ShouldFailValidation()
    {
        var dto = new CreatingOrderDto
        {
            UserId = Guid.NewGuid(),
            Products = new List<CreatingOrderProductDto>
            {
                new() { ProductId = 1, Amount = 1, Price = 9.99 }
            }
        };

        var results = new List<ValidationResult>();
        var context = new ValidationContext(dto);
        var isValid = Validator.TryValidateObject(dto, context, results, true);

        isValid.Should().BeFalse();
        results.Should().Contain(r => r.MemberNames.Contains(nameof(CreatingOrderDto.Status)));
    }
}
