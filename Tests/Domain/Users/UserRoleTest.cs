namespace ShopTex.Tests.Domain.Users;


using Xunit;
using FluentAssertions;
using ShopTex.Domain.Users;
using ShopTex.Domain.Shared;
using System;

public class UserRoleTests
{
    [Theory]
    [InlineData("System", "System Administrator")]
    [InlineData("system administrator", "System Administrator")]
    [InlineData("CLIENT", "Client")]
    [InlineData("Store Administrator", "Store Administrator")]
    [InlineData("store collaborator", "Store Collaborator")]
    public void SelectRoleFromString_ValidInput_ReturnsCorrectRole(string input, string expectedRoleName)
    {
        // Act
        var role = UserRole.SelectRoleFromString(input);

        // Assert
        role.Should().NotBeNull();
        role.RoleName.Should().Be(expectedRoleName);
    }

    [Fact]
    public void SelectRoleFromString_EmptyString_ReturnsNull()
    {
        // Act
        var role = UserRole.SelectRoleFromString("");

        // Assert
        role.Should().BeNull();
    }

    [Fact]
    public void SelectRoleFromString_InvalidRole_ThrowsBusinessRuleValidationException()
    {
        // Act
        Action act = () => UserRole.SelectRoleFromString("invalid-role");

        // Assert
        act.Should().Throw<BusinessRuleValidationException>()
            .WithMessage("*Role field is Invalid*");
    }

    [Fact]
    public void StaticRoles_ShouldHaveExpectedNamesAndDescriptions()
    {
        // Assert
        UserRole.SystemRole.RoleName.Should().Be("System Administrator");
        UserRole.SystemRole.Description.Should().Contain("full access");

        UserRole.UserNRole.RoleName.Should().Be("Client");

        UserRole.StoreAdminRole.RoleName.Should().Be("Store Administrator");
        UserRole.StoreAdminRole.Description.Should().Contain("store-specific");

        UserRole.StoreColabRole.RoleName.Should().Be("Store Collaborator");
        UserRole.StoreColabRole.Description.Should().Contain("limited access");
    }
}