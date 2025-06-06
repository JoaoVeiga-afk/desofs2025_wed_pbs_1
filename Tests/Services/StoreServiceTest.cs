using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using ShopTex.Domain.Shared;
using ShopTex.Domain.Stores;
using ShopTex.Domain.Users;
using ShopTex.Services;
using Xunit;

namespace ShopTex.Tests.Services;

public class StoreServiceTest
{
    private readonly Mock<IUnitOfWork> _unitOfWork = new Mock<IUnitOfWork>();
    private readonly Mock<IStoreRepository> _storeRepository = new Mock<IStoreRepository>();
    private readonly Mock<IUserRepository> _userReporitory = new Mock<IUserRepository>();
    private readonly Mock<IConfiguration> _configuration = new Mock<IConfiguration>();
    private readonly Mock<ILogger<StoreService>> _logger = new Mock<ILogger<StoreService>>();

    private static readonly StoreAddress _testAddress = new StoreAddress("123 Main St", "Metropolis", "StateX", "12345", "Countryland");
    private readonly Store _testStore = new Store("Main Store", _testAddress, "enabled");

    private StoreService CreateService()
    {
        return new StoreService(_unitOfWork.Object, _storeRepository.Object, _userReporitory.Object, _configuration.Object, _logger.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsListOfStoreDtos()
    {
        // Arrange
        var stores = new List<Store> { _testStore };
        _storeRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(stores);
        var service = CreateService();

        // Act
        var result = await service.GetAllAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal("Main Store", result[0].Name);
        Assert.Equal(_testAddress.ToString(), result[0].Address);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingStore_ReturnsStoreDto()
    {
        // Arrange
        _storeRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<StoreId>())).ReturnsAsync(_testStore);
        var service = CreateService();

        // Act
        var result = await service.GetByIdAsync(new StoreId(Guid.NewGuid()));

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Main Store", result?.Name);
        Assert.Equal(_testAddress.ToString(), result?.Address);
    }

    [Fact]
    public async Task GetByIdAsync_StoreNotFound_ReturnsNull()
    {
        // Arrange
        _storeRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<StoreId>())).ReturnsAsync((Store?)null);
        var service = CreateService();

        // Act
        var result = await service.GetByIdAsync(new StoreId(Guid.NewGuid()));

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_ValidStore_ReturnsStoreDto()
    {
        // Arrange
        var dto = new CreatingStoreDto
        {
            Name = "New Store",
            Address = _testAddress,
            Status = "enabled"
        };

        var service = CreateService();

        _storeRepository.Setup(r => r.AddAsync(It.IsAny<Store>())).ReturnsAsync(_testStore);
        _unitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(1);

        // Act
        var result = await service.AddAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dto.Name, result.Name);
        Assert.Equal(dto.Address.ToString(), result.Address);
    }

    [Fact]
    public async Task AddAsync_ThrowsException_ReturnsBusinessRuleValidationException()
    {
        // Arrange
        var dto = new CreatingStoreDto
        {
            Name = "Duplicate Store",
            Address = _testAddress,
            Status = "enabled"
        };

        _storeRepository.Setup(r => r.AddAsync(It.IsAny<Store>())).ThrowsAsync(new Exception("Some DB Error"));
        var service = CreateService();

        // Act & Assert
        await Assert.ThrowsAsync<BusinessRuleValidationException>(() => service.AddAsync(dto));
    }

    private static User CreateUser(string name, string phone, string email, string password, string role)
    {
        var salt = UserService.GeneratePasswordSalt();
        var hashedPassword = Configurations.HashString(password, salt);
        return new User(name, phone, email, hashedPassword, role, salt);
    }

    [Fact]
    public async Task AddStoreColaborator_UserNotFound_ReturnsFalseWithMessage()
    {
        // Arrange
        string email = "missing@example.com";
        string storeId = Guid.NewGuid().ToString();

        _userReporitory.Setup(r => r.FindByEmail(email)).ReturnsAsync((User)null);

        var service = CreateService();

        // Act
        var (success, message) = await service.AddStoreColaborator(storeId, email);

        // Assert
        Assert.False(success);
        Assert.Equal("User not found.", message);
    }

    [Fact]
    public async Task AddStoreColaborator_StoreNotFound_ReturnsFalseWithMessage()
    {
        // Arrange
        string email = "colab@example.com";
        string storeId = Guid.NewGuid().ToString();

        var user = CreateUser("User", "912345678", email, "pass", UserRole.StoreColabRole.RoleName);

        _userReporitory.Setup(r => r.FindByEmail(email)).ReturnsAsync(user);
        _storeRepository.Setup(r => r.FindById(storeId)).ReturnsAsync((Store)null);

        var service = CreateService();

        // Act
        var (success, message) = await service.AddStoreColaborator(storeId, email);

        // Assert
        Assert.False(success);
        Assert.Equal("Store not found.", message);
    }

    [Fact]
    public async Task AddStoreColaborator_UserWithInvalidRole_ReturnsFalseWithMessage()
    {
        // Arrange
        string email = "colab@example.com";
        string storeId = Guid.NewGuid().ToString();

        var user = CreateUser("User", "912345678", email, "pass", UserRole.SystemRole.RoleName);

        _userReporitory.Setup(r => r.FindByEmail(email)).ReturnsAsync(user);
        _storeRepository.Setup(r => r.FindById(storeId)).ReturnsAsync(_testStore);

        var service = CreateService();

        // Act
        var (success, message) = await service.AddStoreColaborator(storeId, email);

        // Assert
        Assert.False(success);
        Assert.Equal("User does not have the correct role for store assignment.", message);
    }

    [Fact]
    public async Task AddStoreColaborator_SuccessfulAssignment_ReturnsTrueWithMessage()
    {
        // Arrange
        string email = "colab@example.com";
        string storeId = Guid.NewGuid().ToString();

        var user = CreateUser("User", "912345678", email, "pass", UserRole.StoreColabRole.RoleName);

        _userReporitory.Setup(r => r.FindByEmail(email)).ReturnsAsync(user);
        _storeRepository.Setup(r => r.FindById(storeId)).ReturnsAsync(_testStore);

        var service = CreateService();

        // Act
        var (success, message) = await service.AddStoreColaborator(storeId, email);

        // Assert
        Assert.True(success);
        Assert.Equal("Collaborator added to store successfully.", message);
    }
}
