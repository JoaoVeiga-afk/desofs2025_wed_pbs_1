namespace ShopTex.Tests.Domain.Users;

using Xunit;
using ShopTex.Domain.Users;
using System;

public class UserIdTests
{
    [Fact]
    public void Constructor_WithGuid_ShouldStoreCorrectValue()
    {
        // Arrange
        Guid guid = Guid.NewGuid();

        // Act
        var userId = new UserId(guid);

        // Assert
        Assert.Equal(guid, userId.AsGuid());
        Assert.Equal(guid.ToString(), userId.AsString());
    }

    [Fact]
    public void Constructor_WithValidGuidString_ShouldStoreCorrectValue()
    {
        // Arrange
        Guid expectedGuid = Guid.NewGuid();
        string guidString = expectedGuid.ToString();

        // Act
        var userId = new UserId(guidString);

        // Assert
        Assert.Equal(expectedGuid, userId.AsGuid());
        Assert.Equal(guidString, userId.AsString());
    }

    [Fact]
    public void AsString_ShouldReturnCorrectStringRepresentation()
    {
        // Arrange
        Guid guid = Guid.NewGuid();
        var userId = new UserId(guid);

        // Act
        string str = userId.AsString();

        // Assert
        Assert.Equal(guid.ToString(), str);
    }

    [Fact]
    public void AsGuid_ShouldReturnCorrectGuid()
    {
        // Arrange
        Guid guid = Guid.NewGuid();
        var userId = new UserId(guid);

        // Act
        Guid result = userId.AsGuid();

        // Assert
        Assert.Equal(guid, result);
    }

    [Fact]
    public void Constructor_WithInvalidString_ShouldThrowFormatException()
    {
        // Arrange
        string invalidGuid = "not-a-guid";

        // Act & Assert
        Assert.Throws<FormatException>(() => new UserId(invalidGuid));
    }
}
