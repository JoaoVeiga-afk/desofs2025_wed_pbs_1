namespace ShopTex.Tests.Domain.Users;

using Xunit;
using System;
using ShopTex.Domain.Users;
using ShopTex.Domain.Shared; // For BusinessRuleValidationException

public class UserStatusTests
{
    [Fact]
    public void Constructor_WithBoolTrue_ShouldSetValueTrue()
    {
        var status = new UserStatus(true);

        Assert.True(status.Value);
        Assert.Equal("enabled", status.ToString());
    }

    [Fact]
    public void Constructor_WithBoolFalse_ShouldSetValueFalse()
    {
        var status = new UserStatus(false);

        Assert.False(status.Value);
        Assert.Equal("disabled", status.ToString());
    }

    [Fact]
    public void Constructor_WithStringEnabled_ShouldSetValueTrue()
    {
        var status = new UserStatus("enabled");

        Assert.True(status.Value);
        Assert.Equal("enabled", status.ToString());
    }

    [Fact]
    public void Constructor_WithStringDisabled_ShouldSetValueFalse()
    {
        var status = new UserStatus("disabled");

        Assert.False(status.Value);
        Assert.Equal("disabled", status.ToString());
    }

    [Fact]
    public void Constructor_WithInvalidString_ShouldThrowBusinessRuleValidationException()
    {
        Assert.Throws<BusinessRuleValidationException>(() =>
            new UserStatus("invalid_status"));
    }

    [Theory]
    [InlineData("Disabled")]
    [InlineData("Enable")]
    public void Constructor_WithIncorrectCase_ShouldThrowBusinessRuleValidationException(string input)
    {
        Assert.Throws<BusinessRuleValidationException>(() =>
            new UserStatus(input));
    }
}
