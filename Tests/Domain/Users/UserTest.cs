using ShopTex.Domain.Users;

namespace ShopTex.Tests.Domain.Users;
using Xunit;
using FluentAssertions;
    
public class UserTests
{
    [Fact]
    public void Constructor_WithValidArguments_ShouldInitializeProperties()
    {
        // Arrange
        string name = "John Doe";
        string phone = "123456789";
        string email = "john@example.com";
        string password = "SecurePass!";
        string role = "system";
        string status = "enabled";
        byte[] salt = new byte[] { 1, 2, 3 };

        // Act
        var user = new User(name, phone, email, password, role, status, salt);

        // Assert
        user.Id.Should().NotBeNull();
        user.Name.Should().Be(name);
        user.Phone.Should().Be(phone);
        user.Email.Value.Should().Be(email);
        user.Password.Should().Be(password);
        user.Role.Should().Be(UserRole.SelectRoleFromString(role));
        user.Salt.Should().BeEquivalentTo(salt); 
    }

    [Fact]
    public void Constructor_WithNullRole_ShouldSetRoleToNull()
    {
        // Arrange
        string name = "Jane Doe";
        string phone = "987654321";
        string email = "jane@example.com";
        string password = "AnotherPass!";
        string? role = null;
        string status = "enabled";
        byte[] salt = new byte[] { 9, 8, 7 };

        // Act
        var user = new User(name, phone, email, password, role, status, salt);

        // Assert
        user.Role.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithIdString_ShouldParseIdCorrectly()
    {
        // Arrange
        string id = Guid.NewGuid().ToString();
        string name = "Alex";
        string phone = "555555555";
        string email = "alex@example.com";
        string password = "Pass123!";
        string role = "Client";
        string status = "enabled";
        byte[] salt = new byte[] { 0, 0, 0 };

        // Act
        var user = new User(id, name, phone, email, password, role, status, salt);

        // Assert
        user.Id.Value.ToString().Should().Be(id);
        user.Name.Should().Be(name);
        user.Email.Value.Should().Be(email);
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenEmailIsInvalid()
    {
        // Arrange
        string invalidEmail = "not-an-email";

        // Act
        Action act = () => new User("Name", "Phone", invalidEmail, "pass", "Admin", "Active", new byte[0]);

        // Assert
        act.Should().Throw<Exception>()
           .WithMessage("*invalid email*");
    }
}