using System;
using FluentAssertions;
using ShopTex.Domain.Orders;
using ShopTex.Domain.Shared;
using Xunit;

namespace ShopTex.Tests.Domain.Orders;

public class OrderStatusTest
{
    [Theory]
    [InlineData("pending")]
    [InlineData("processing")]
    [InlineData("delivered")]
    [InlineData("cancelled")]
    public void Constructor_WithValidStatus_ShouldSetValue(string status)
    {
        // Act
        var orderStatus = new OrderStatus(status);

        // Assert
        orderStatus.Value.Should().Be(status.ToLower());
        orderStatus.ToString().Should().Be(status.ToLower());
    }

    [Theory]
    [InlineData("shipped")]
    [InlineData("invalid")]
    [InlineData("")]
    [InlineData("completed")]
    public void Constructor_WithInvalidStatus_ShouldThrowException(string invalidStatus)
    {
        // Act
        Action act = () => new OrderStatus(invalidStatus);

        // Assert
        act.Should().Throw<BusinessRuleValidationException>()
            .WithMessage("Invalid order status*");
    }
}