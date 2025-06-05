namespace ShopTex.Tests.Domain.Stores;

using Xunit;
using ShopTex.Domain.Stores;
using System;

public class StoreIdTests
{
    [Fact]
    public void Constructor_WithGuid_ShouldStoreCorrectValue()
    {
        // Arrange
        Guid guid = Guid.NewGuid();

        // Act
        var storeId = new StoreId(guid);

        // Assert
        Assert.Equal(guid, storeId.AsGuid());
        Assert.Equal(guid.ToString(), storeId.AsString());
    }

    [Fact]
    public void Constructor_WithValidGuidString_ShouldStoreCorrectValue()
    {
        // Arrange
        Guid expectedGuid = Guid.NewGuid();
        string guidString = expectedGuid.ToString();

        // Act
        var storeId = new StoreId(guidString);

        // Assert
        Assert.Equal(expectedGuid, storeId.AsGuid());
        Assert.Equal(guidString, storeId.AsString());
    }

    [Fact]
    public void AsString_ShouldReturnCorrectStringRepresentation()
    {
        // Arrange
        Guid guid = Guid.NewGuid();
        var storeId = new StoreId(guid);

        // Act
        string str = storeId.AsString();

        // Assert
        Assert.Equal(guid.ToString(), str);
    }

    [Fact]
    public void AsGuid_ShouldReturnCorrectGuid()
    {
        // Arrange
        Guid guid = Guid.NewGuid();
        var storeId = new StoreId(guid);

        // Act
        Guid result = storeId.AsGuid();

        // Assert
        Assert.Equal(guid, result);
    }

    [Fact]
    public void Constructor_WithInvalidString_ShouldThrowFormatException()
    {
        // Arrange
        string invalidGuid = "not-a-guid";

        // Act & Assert
        Assert.Throws<FormatException>(() => new StoreId(invalidGuid));
    }
}
