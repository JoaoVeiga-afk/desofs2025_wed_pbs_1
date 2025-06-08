namespace ShopTex.Tests.Domain.Products;

using Xunit;
using ShopTex.Domain.Products;
using System;

public class ProductIdTests
{
    [Fact]
    public void Constructor_WithGuid_ShouldProductCorrectValue()
    {
        // Arrange
        Guid guid = Guid.NewGuid();

        // Act
        var productId = new ProductId(guid);

        // Assert
        Assert.Equal(guid, productId.AsGuid());
        Assert.Equal(guid.ToString(), productId.AsString());
    }

    [Fact]
    public void Constructor_WithValidGuidString_ShouldProductCorrectValue()
    {
        // Arrange
        Guid expectedGuid = Guid.NewGuid();
        string guidString = expectedGuid.ToString();

        // Act
        var productId = new ProductId(guidString);

        // Assert
        Assert.Equal(expectedGuid, productId.AsGuid());
        Assert.Equal(guidString, productId.AsString());
    }

    [Fact]
    public void AsString_ShouldReturnCorrectStringRepresentation()
    {
        // Arrange
        Guid guid = Guid.NewGuid();
        var productId = new ProductId(guid);

        // Act
        string str = productId.AsString();

        // Assert
        Assert.Equal(guid.ToString(), str);
    }

    [Fact]
    public void AsGuid_ShouldReturnCorrectGuid()
    {
        // Arrange
        Guid guid = Guid.NewGuid();
        var productId = new ProductId(guid);

        // Act
        Guid result = productId.AsGuid();

        // Assert
        Assert.Equal(guid, result);
    }

    [Fact]
    public void Constructor_WithInvalidString_ShouldThrowFormatException()
    {
        // Arrange
        string invalidGuid = "not-a-guid";

        // Act & Assert
        Assert.Throws<FormatException>(() => new ProductId(invalidGuid));
    }
}
