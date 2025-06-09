namespace ShopTex.Tests.Domain.Stores;

using Xunit;
using System;
using ShopTex.Domain.Stores;
using ShopTex.Domain.Shared; // For BusinessRuleValidationException

public class StoreStatusTests
{
    [Fact]
    public void Constructor_WithBoolTrue_ShouldSetValueTrue()
    {
        var status = new StoreStatus(true);

        Assert.True(status.Value);
        Assert.Equal("enabled", status.ToString());
    }

    [Fact]
    public void Constructor_WithBoolFalse_ShouldSetValueFalse()
    {
        var status = new StoreStatus(false);

        Assert.False(status.Value);
        Assert.Equal("disabled", status.ToString());
    }

    [Fact]
    public void Constructor_WithStringEnabled_ShouldSetValueTrue()
    {
        var status = new StoreStatus("enabled");

        Assert.True(status.Value);
        Assert.Equal("enabled", status.ToString());
    }

    [Fact]
    public void Constructor_WithStringDisabled_ShouldSetValueFalse()
    {
        var status = new StoreStatus("disabled");

        Assert.False(status.Value);
        Assert.Equal("disabled", status.ToString());
    }

    [Fact]
    public void Constructor_WithInvalidString_ShouldThrowBusinessRuleValidationException()
    {
        Assert.Throws<BusinessRuleValidationException>(() =>
            new StoreStatus("invalid_status"));
    }

    [Theory]
    [InlineData("Disabled")]
    [InlineData("Enable")]
    public void Constructor_WithIncorrectCase_ShouldThrowBusinessRuleValidationException(string input)
    {
        Assert.Throws<BusinessRuleValidationException>(() =>
            new StoreStatus(input));
    }
}
