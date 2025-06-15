using System;
using Xunit;
using ShopTex.Domain.Stores;

public class AddClientDtoTests
{
    [Fact]
    public void AddClientDto_ShouldSetStoreIdCorrectly()
    {
        // Arrange
        var storeId = Guid.NewGuid();

        // Act
        var dto = new AddClientDto { StoreId = storeId };

        // Assert
        Assert.Equal(storeId, dto.StoreId);
    }

    [Fact]
    public void AddClientDto_ShouldSetUserIdCorrectly()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var dto = new AddClientDto { UserId = userId };

        // Assert
        Assert.Equal(userId, dto.UserId);
    }

    [Fact]
    public void AddClientDto_UserIdShouldBeNullable()
    {
        // Arrange
        var dto = new AddClientDto { StoreId = Guid.NewGuid() };

        // Act & Assert
        Assert.Null(dto.UserId);
    }

    [Fact]
    public void AddClientDto_ShouldHandleNullUserId()
    {
        // Arrange
        var dto = new AddClientDto { StoreId = Guid.NewGuid(), UserId = null };

        // Act & Assert
        Assert.Null(dto.UserId);
    }

    [Fact]
    public void AddClientDto_ShouldHandleDefaultCtorValues()
    {
        // Arrange & Act
        var dto = new AddClientDto();

        // Assert
        Assert.Equal(Guid.Empty, dto.StoreId);
        Assert.Null(dto.UserId);
    }
}