namespace ShopTex.Tests.Domain.Products;

using Xunit;
using ShopTex.Domain.Products;
using ShopTex.Domain.Shared; // For BusinessRuleValidationException

public class ProductStatusTests
{
    [Fact]
    public void Constructor_WithBoolTrue_ShouldSetValueTrue()
    {
        var status = new ProductStatus(true);

        Assert.True(status.Value);
        Assert.Equal("enabled", status.ToString());
    }

    [Fact]
    public void Constructor_WithBoolFalse_ShouldSetValueFalse()
    {
        var status = new ProductStatus(false);

        Assert.False(status.Value);
        Assert.Equal("disabled", status.ToString());
    }

    [Fact]
    public void Constructor_WithStringEnabled_ShouldSetValueTrue()
    {
        var status = new ProductStatus("enabled");

        Assert.True(status.Value);
        Assert.Equal("enabled", status.ToString());
    }

    [Fact]
    public void Constructor_WithStringDisabled_ShouldSetValueFalse()
    {
        var status = new ProductStatus("disabled");

        Assert.False(status.Value);
        Assert.Equal("disabled", status.ToString());
    }

    [Fact]
    public void Constructor_WithInvalidString_ShouldThrowBusinessRuleValidationException()
    {
        Assert.Throws<BusinessRuleValidationException>(() =>
            new ProductStatus("invalid_status"));
    }

    [Theory]
    [InlineData("Disabled")]
    [InlineData("Enable")]
    public void Constructor_WithIncorrectCase_ShouldThrowBusinessRuleValidationException(string input)
    {
        Assert.Throws<BusinessRuleValidationException>(() =>
            new ProductStatus(input));
    }
}
