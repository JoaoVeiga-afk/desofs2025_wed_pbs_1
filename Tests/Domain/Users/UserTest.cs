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
        var user = new User(name, phone, email, Configurations.HashString(password, salt), role, salt);

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
        var user = new User(name, phone, email, Configurations.HashString(password, salt), role, salt);

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
        var user = new User(id, name, phone, email, Configurations.HashString(password, salt), role, status, salt);

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

    [Fact]
    public void EnableUser_WhenStatusIsDisabled_ShouldSetStatusToEnabled_AndReturnTrue()
    {
        // Arrange
        string id = Guid.NewGuid().ToString();
        string name = "John Doe";
        string phone = "123456789";
        string email = "john@example.com";
        string password = "SecurePass!";
        string role = Configurations.STORE_COLAB_ROLE_NAME;
        string status = "disabled";
        byte[] salt = new byte[] { 1, 2, 3 };

        var user = new User(id, name, phone, email, Configurations.HashString(password, salt), role, status, salt);

        // Act
        var result = user.EnableUser();

        // Assert
        result.Should().BeTrue();
        user.Status.ToString().Should().Be("enabled");
    }

    [Fact]
    public void EnableUser_WhenStatusIsEnabled_ShouldReturnFalse_AndStatusUnchanged()
    {
        // Arrange
        string id = Guid.NewGuid().ToString();
        string name = "John Doe";
        string phone = "123456789";
        string email = "john@example.com";
        string password = "SecurePass!";
        string role = Configurations.STORE_COLAB_ROLE_NAME;
        string status = "enabled";
        byte[] salt = new byte[] { 1, 2, 3 };

        var user = new User(id, name, phone, email, Configurations.HashString(password, salt), role, status, salt);

        // Act
        var result = user.EnableUser();

        // Assert
        result.Should().BeFalse();
        user.Status.ToString().Should().Be("enabled");
    }

    [Fact]
    public void DisableUser_WhenStatusIsEnabled_ShouldSetStatusToDisabled_AndReturnTrue()
    {
        // Arrange
        string id = Guid.NewGuid().ToString();
        string name = "John Doe";
        string phone = "123456789";
        string email = "john@example.com";
        string password = "SecurePass!";
        string role = Configurations.STORE_COLAB_ROLE_NAME;
        string status = "enabled";
        byte[] salt = new byte[] { 1, 2, 3 };

        var user = new User(id, name, phone, email, Configurations.HashString(password, salt), role, status, salt);

        // Act
        var result = user.DisableUser();

        // Assert
        result.Should().BeTrue();
        user.Status.ToString().Should().Be("disabled");
    }

    [Fact]
    public void DisableUser_WhenStatusIsDisabled_ShouldReturnFalse_AndStatusUnchanged()
    {
        // Arrange
        string id = Guid.NewGuid().ToString();
        string name = "John Doe";
        string phone = "123456789";
        string email = "john@example.com";
        string password = "SecurePass!";
        string role = Configurations.STORE_COLAB_ROLE_NAME;
        string status = "disabled";
        byte[] salt = new byte[] { 1, 2, 3 };

        var user = new User(id, name, phone, email, Configurations.HashString(password, salt), role, status, salt);

        // Act
        var result = user.DisableUser();

        // Assert
        result.Should().BeFalse();
        user.Status.ToString().Should().Be("disabled");
    }
    
    [Fact]
    public void SetStore_WithValidRole_ShouldSetStoreAndReturnTrue()
    {
        // Arrange
        var user = new User("John Doe", "123456789", "john@store.com", "pass", Configurations.STORE_ADMIN_ROLE_NAME, new byte[] { 1, 2, 3 });
        string storeId = Guid.NewGuid().ToString();

        // Act
        var result = user.SetStore(storeId);

        // Assert
        result.Should().BeTrue();
        user.Store.Value.ToString().Should().Be(storeId);
    }

    [Fact]
    public void SetStore_WithInvalidRole_ShouldReturnFalseAndNotSetStore()
    {
        // Arrange
        var user = new User("John Doe", "123456789", "john@client.com", "pass", "client", new byte[] { 1, 2, 3 });
        string storeId = Guid.NewGuid().ToString();

        // Act
        var result = user.SetStore(storeId);

        // Assert
        result.Should().BeFalse();
        user.Store.Should().BeNull();
    }

    [Fact]
    public void ChangeRole_ToClientRole_ShouldRemoveStore()
    {
        // Arrange
        var user = new User("John Doe", "123456789", "john@store.com", "pass", "store administrator", new byte[] { 1, 2, 3 });
        user.Role.RoleName.Should().Be(Configurations.STORE_ADMIN_ROLE_NAME);

        var storeId = Guid.NewGuid().ToString();
        user.SetStore(storeId);
        user.Store.Should().NotBeNull();

        // Act
        user.ChangeRole(UserRole.UserNRole);

        // Assert
        user.Role.RoleName.Should().Be(Configurations.USER_ROLE_NAME);
        user.Store.Should().BeNull();
    }

    [Fact]
    public void ChangeRole_ToStoreAdminRole_ShouldKeepStore()
    {
        // Arrange
        var user = new User("John Doe", "123456789", "john@store.com", "pass", Configurations.STORE_ADMIN_ROLE_NAME, new byte[] { 1, 2, 3 });
        user.Role.RoleName.Should().Be(Configurations.STORE_ADMIN_ROLE_NAME);

        var storeId = Guid.NewGuid().ToString();
        user.SetStore(storeId);
        user.Store.Should().NotBeNull();

        // Act
        user.ChangeRole(UserRole.StoreAdminRole);

        // Assert
        user.Role.RoleName.Should().Be(Configurations.STORE_ADMIN_ROLE_NAME);
        user.Store.Should().NotBeNull();
        user.Store.Value.ToString().Should().Be(storeId);
    }

}