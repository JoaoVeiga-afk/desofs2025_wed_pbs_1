using Org.BouncyCastle.Math.EC;
using ShopTex.Domain.Users;
using ShopTex;
using Xunit;
using FluentAssertions;

namespace ShopTex.Tests.Domain.Users;
    
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
        byte[] salt = new byte[] { 1, 2, 3 };

        // Act
        var user = new User(name, phone, email, Configurations.HashString(password,salt), role, salt);

        // Assert
        user.Id.Should().NotBeNull();
        user.Name.Should().Be(name);
        user.Phone.Should().Be(phone);
        user.Email.Value.Should().Be(email);
        user.Role.Should().Be(UserRole.SelectRoleFromString(role));
        user.VerifyPassword(password).Should().BeTrue();
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
        byte[] salt = new byte[] { 9, 8, 7 };

        // Act
        var user = new User(name, phone, email, Configurations.HashString(password,salt), role, salt);

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
        var user = new User(id, name, phone, email, Configurations.HashString(password,salt), role, status, salt);

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
        Action act = () => new User("Name", "Phone", invalidEmail, "pass", "Admin", new byte[0]);

        // Assert
        act.Should().Throw<Exception>()
           .WithMessage("*invalid email*");
    }

    [Fact]
    public void VerifyPassword_ShouldReturnTrue_WhenPasswordMatches()
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

        var user = new User(id, name, phone, email, Configurations.HashString(password, salt), role, status, salt);

        // Act
        bool result = user.VerifyPassword(password);
        
        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_ShouldReturnFalse_WhenPasswordDoesNotMatch()
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

        var user = new User(id, name, phone, email, Configurations.HashString(password, salt), role, status, salt);

        // Act
        bool result = user.VerifyPassword("not-the-password");
        
        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void VerifyUserWithSytemRole_ShouldSetStatusToDisabled()
    {
        // Arrange
        string name = "System Admin";
        string phone = "123456789";
        string email = "alex@example.com";
        string password = "SecurePass!";
        string role = Configurations.SYS_ADMIN_ROLE_NAME;
        byte[] salt = new byte[] { 1, 2, 3 };
        
        // Act
        var user = new User(name, phone, email, Configurations.HashString(password, salt), role, salt);

        // Assert
        user.Status.ToString().Should().Be("disabled");
    }

    [Fact]
    public void VerifyUserWithStoreAdminRole_ShouldSetStatusToDisabled()
    {
        // Arrange
        string name = "Store Admin";
        string phone = "123456789";
        string email = "alex@example.com";
        string password = "SecurePass!";
        string role = Configurations.STORE_ADMIN_ROLE_NAME;
        byte[] salt = new byte[] { 1, 2, 3 };
        
        // Act
        var user = new User(name, phone, email, Configurations.HashString(password, salt), role, salt);
        
        // Assert
        user.Status.ToString().Should().Be("disabled");
    }

    [Fact]
    public void VerifyUserWithStoreColabRole_ShouldSetStatusToEnabled()
    {
        // Arrange
        string name = "Store Collaborator";
        string phone = "123456789";
        string email = "alex@example.com";
        string password = "SecurePass!";
        string role = Configurations.STORE_COLAB_ROLE_NAME;
        byte[] salt = new byte[] { 1, 2, 3 };
        
        // Act
        var user = new User(name, phone, email, Configurations.HashString(password, salt), role, salt);
        
        // Assert
        user.Status.ToString().Should().Be("enabled");
    }
}