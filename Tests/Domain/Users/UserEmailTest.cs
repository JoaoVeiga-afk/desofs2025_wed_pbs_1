namespace ShopTex.Tests.Domain.Users;

using Xunit;
using ShopTex.Domain.Users;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

public class UserEmailTests
{
    [Fact]
    public void Constructor_WithValidEmail_ShouldSetValue()
    {
        // Arrange
        string validEmail = "test@example.com";

        // Act
        var email = new UserEmail(validEmail);

        // Assert
        Assert.Equal(validEmail, email.Value);
    }

    [Fact]
    public void Constructor_WithInvalidEmail_Should_ThrowExeption()
    {
        // Arrange
        string invalidEmail = "not-an-email";

        // Act
        var exception = Record.Exception(() => new UserEmail(invalidEmail));
        // Assert
        Assert.NotNull(exception);
        Assert.IsType<ValidationException>(exception);
        Assert.Equal("Invalid email format", exception.Message);

    }

    [Fact]
    public void Constructor_WithEmptyEmail_ShouldThrowException()
    {
        // Arrange
        string emptyEmail = "";

        // Act
        var exception = Record.Exception(() => new UserEmail(emptyEmail));

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<ValidationException>(exception);
        Assert.Equal("Invalid email format", exception.Message);
    }

    [Fact]
    public void Email_WithValidFormat_ShouldPassManualValidation()
    {
        // Arrange
        var email = new UserEmail("john.doe@domain.com");

        // Act
        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(email);
        bool isValid = Validator.TryValidateObject(email, context, validationResults, true);

        // Assert
        Assert.True(isValid);
        Assert.Empty(validationResults);
    }
}

