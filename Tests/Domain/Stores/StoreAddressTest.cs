using System;
using FluentAssertions;
using ShopTex.Domain.Stores;
using Xunit;
using System.ComponentModel.DataAnnotations;

namespace ShopTex.Tests.Domain.Stores;

public class StoreAddressTest
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldInitializeCorrectly()
    {
        // Arrange
        var street = "123 Main St";
        var city = "Metropolis";
        var state = "StateX";
        var zip = "12345";
        var country = "Countryland";

        // Act
        var address = new StoreAddress(street, city, state, zip, country);

        // Assert
        address.Street.Should().Be(street);
        address.City.Should().Be(city);
        address.State.Should().Be(state);
        address.ZipCode.Should().Be(zip);
        address.Country.Should().Be(country);
    }

    [Theory]
    [InlineData(null, "City", "State", "1234", "Country", "street")]
    [InlineData("Street", null, "State", "1234", "Country", "city")]
    [InlineData("Street", "City", null, "1234", "Country", "state")]
    [InlineData("Street", "City", "State", null, "Country", "zipCode")]
    [InlineData("Street", "City", "State", "1234", null, "country")]
    public void Constructor_WithNullArguments_ShouldThrowArgumentNullException(string? street, string? city, string? state, string? zip, string? country, string paramName)
    {
        // Act
        Action act = () => new StoreAddress(street, city, state, zip, country);

        // Assert
        act.Should().Throw<ArgumentNullException>().Where(e => e.ParamName == paramName);
    }

    [Theory]
    [InlineData("12345678901", "City", "State", "12345678901", "Country")] // Zip > 10
    [InlineData("Street", "City", "State", "123", "Country")]              // Zip < 4
    [InlineData("", "City", "State", "1234", "Country")]                   // Street too short
    [InlineData("Street", "", "State", "1234", "Country")]                 // City too short
    public void Constructor_WithInvalidLengths_ShouldThrowValidationException(string street, string city, string state, string zip, string country)
    {
        // Act
        Action act = () => new StoreAddress(street, city, state, zip, country);

        // Assert
        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void Equals_ShouldReturnTrue_ForIdenticalValues()
    {
        // Arrange
        var addr1 = new StoreAddress("Street", "City", "State", "1234", "Country");
        var addr2 = new StoreAddress("Street", "City", "State", "1234", "Country");

        // Assert
        addr1.Equals(addr2).Should().BeTrue();
        addr1.Equals((object)addr2).Should().BeTrue();
        addr1.GetHashCode().Should().Be(addr2.GetHashCode());
    }

    [Fact]
    public void Equals_ShouldReturnFalse_ForDifferentValues()
    {
        var addr1 = new StoreAddress("Street", "City", "State", "1234", "Country");
        var addr2 = new StoreAddress("Other St", "City", "State", "1234", "Country");

        addr1.Equals(addr2).Should().BeFalse();
    }

    [Fact]
    public void ToString_ShouldReturnFormattedAddress()
    {
        var address = new StoreAddress("1 Infinite Loop", "Cupertino", "CA", "95014", "USA");

        address.ToString().Should().Be("1 Infinite Loop, Cupertino, CA 95014, USA");
    }
}
