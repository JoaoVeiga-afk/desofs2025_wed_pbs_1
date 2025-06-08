using System;
using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using ShopTex.Domain.Products;
using ShopTex.Domain.Shared;
using Xunit;

namespace ShopTex.Tests.Domain.Products;

public class ProductTest
{
    [Fact]
    public void Constructor_WithValidArguments_ShouldInitializeProperties()
    {
        // Arrange
        var name = "Laptop";
        var description = "A powerful gaming laptop";
        var price = 1500.0;
        var category = "Electronics";
        var status = "enabled";
        var storeId = Guid.NewGuid().ToString();

        // Act
        var product = new Product(name, description, price, category, status, storeId);

        // Assert
        product.Id.Should().NotBeNull();
        product.Name.Should().Be(name);
        product.Description.Should().Be(description);
        product.Price.Should().Be(price);
        product.Category.Should().Be(category);
        product.Status.ToString().Should().Be(status);
        product.StoreId.Value.ToString().Should().Be(storeId);
    }

    [Fact]
    public void Constructor_WithIdString_ShouldParseIdCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid().ToString();
        var name = "Phone";
        var description = "Smartphone";
        var price = 999.99f;
        var category = "Electronics";
        var status = "disabled";
        var storeId = Guid.NewGuid().ToString();

        // Act
        var product = new Product(id, name, description, price, category, status, storeId);

        // Assert
        product.Id.Value.ToString().Should().Be(id);
        product.Name.Should().Be(name);
        product.Description.Should().Be(description);
        product.Price.Should().Be((double)price);
        product.Category.Should().Be(category);
        product.Status.ToString().Should().Be(status);
        product.StoreId.Value.ToString().Should().Be(storeId);
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("ENABLED")]
    [InlineData("")]
    [InlineData(null)]
    public void Constructor_ShouldThrowBusinessRuleValidationException_WhenStatusIsInvalid(string invalidStatus)
    {
        // Arrange
        var storeId = Guid.NewGuid().ToString();

        // Act
        Action act = () => new Product("Product", "Test", 10.0, "Test", invalidStatus, storeId);

        // Assert
        act.Should().Throw<BusinessRuleValidationException>();
    }

    [Fact]
    public void Name_ShouldNotExceedMaxLength()
    {
        // Arrange
        var longName = new string('X', 101);
        var storeId = Guid.NewGuid().ToString();

        // Act
        Action act = () => new Product(longName, "Desc", 10.0, "Cat", "enabled", storeId);

        // Assert
        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void Description_ShouldNotExceedMaxLength()
    {
        // Arrange
        var longDescription = new string('D', 251);
        var storeId = Guid.NewGuid().ToString();

        // Act
        Action act = () => new Product("Product", longDescription, 5.0, "Cat", "enabled", storeId);

        // Assert
        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void Constructor_ShouldThrowValidationException_WhenRequiredFieldsAreMissing()
    {
        // Act
        Action act = () => new Product(null, null, 0.0, null, "enabled", null);

        // Assert
        act.Should().Throw<NullReferenceException>();
    }
}
