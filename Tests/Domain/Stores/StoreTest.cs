using System;
using FluentAssertions;
using ShopTex.Domain.Stores;
using Xunit;

namespace ShopTex.Tests.Domain.Stores;

public class StoreTest
{
    [Fact]
    public void Constructor_WithValidArguments_ShouldInitializeProperties()
    {
        // Arrange
        var name = "Super Store";
        var address = new StoreAddress("Main St", "123", "Cityville", "12345", "Country");
        var status = "enabled";

        // Act
        var store = new Store(name, address, status);

        // Assert
        store.Id.Should().NotBeNull();
        store.Name.Should().Be(name);
        store.Address.Should().Be(address);
        store.Status.ToString().Should().Be(status);
    }

    [Fact]
    public void Constructor_WithIdString_ShouldParseIdCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid().ToString();
        var name = "Test Store";
        var address = new StoreAddress("Test St", "99", "Test City", "99999", "Testland");
        var status = "disabled";

        // Act
        var store = new Store(id, name, address, status);

        // Assert
        store.Id.Value.ToString().Should().Be(id);
        store.Name.Should().Be(name);
        store.Address.Should().Be(address);
        store.Status.ToString().Should().Be(status);
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenStatusIsInvalid()
    {
        // Arrange
        var invalidStatus = "not-a-status";
        var address = new StoreAddress("Invalid St", "0", "Nowhere", "00000", "Neverland");

        // Act
        Action act = () => new Store("Invalid Store", address, invalidStatus);

        // Assert
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void Name_ShouldNotExceedMaxLength()
    {
        // Arrange
        var longName = new string('A', 101); // 101 chars
        var address = new StoreAddress("Too Long", "1", "BigCity", "11111", "Land");
        var status = "enabled";

        // Act
        Action act = () => new Store(longName, address, status);

        // Assert
        act.Should().Throw<Exception>();
    }
}
