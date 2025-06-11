using Moq;
using ShopTex.Domain.Shared;
using ShopTex.Domain.Users;
using ShopTex.Services;
using Xunit;

namespace ShopTex.Tests.Services;

public class AuthenticationServiceTest
{
    private readonly Mock<IUnitOfWork> _unitOfWork = new Mock<IUnitOfWork>();
    private readonly Mock<IConfiguration> _configuration = new Mock<IConfiguration>();
    private readonly Mock<IUserRepository> _userRepository = new Mock<IUserRepository>();
    private readonly Mock<ILogger<UserService>> _logger = new Mock<ILogger<UserService>>();
    
    private static readonly byte[] Salt = UserService.GeneratePasswordSalt();
    private readonly User _testUser = new User("Testuser", "912345678", "testuser@example.com", Configurations.HashString("passwd", Salt), UserRole.UserNRole.RoleName, Salt);
    


    [Fact]
    public async Task HasPermissionTest_Pass()
    {
        _userRepository.Setup(repository => repository.FindByEmail(It.IsAny<string>())).ReturnsAsync(_testUser);

        var service = new AuthenticationService(_unitOfWork.Object, _userRepository.Object, _configuration.Object,
            _logger.Object);

        var result = await service.hasPermission("testuser@example.com",
            new List<UserRole> { UserRole.UserNRole, UserRole.StoreColabRole });

        Assert.True(result);
    }

    [Fact]
    public async Task HasPermissionTest_Fail()
    {
        _userRepository.Setup(repository => repository.FindByEmail(It.IsAny<string>())).ReturnsAsync(_testUser);

        var service = new AuthenticationService(_unitOfWork.Object, _userRepository.Object, _configuration.Object,
            _logger.Object);

        var result = await service.hasPermission("testuser@example.com",
            new List<UserRole> { UserRole.StoreAdminRole, UserRole.SystemRole });

        Assert.False(result);
    }

    [Fact]
    public async Task HasPermissionTest_UserNotFound()
    {
        _userRepository.Setup(repository => repository.FindByEmail(It.IsAny<string>())).ReturnsAsync((User)null);

        var service = new AuthenticationService(_unitOfWork.Object, _userRepository.Object, _configuration.Object,
            _logger.Object);

        var result = await service.hasPermission("testuser@example.com",
            new List<UserRole> { UserRole.UserNRole, UserRole.StoreColabRole });

        Assert.False(result);
    }

    [Fact]
    public async Task HasPermissionTest_UserHasNoRole()
    {
        var testUser2 = new User("Testuser", "912345678", "testuser@example.com",
            Configurations.HashString("passwd", Salt), null, Salt);

        _userRepository.Setup(repository => repository.FindByEmail(It.IsAny<string>())).ReturnsAsync(testUser2);

        var service = new AuthenticationService(_unitOfWork.Object, _userRepository.Object, _configuration.Object,
            _logger.Object);

        var result = await service.hasPermission("testuser@example.com",
            new List<UserRole> { UserRole.UserNRole, UserRole.StoreColabRole });

        Assert.False(result);
    }
    
    [Fact]
    public async Task ManagesStore_UserNotFound_ShouldReturnFalse()
    {
        _userRepository.Setup(r => r.FindByEmail("notfound@example.com"))
            .ReturnsAsync((User)null);

        var service = new AuthenticationService(_unitOfWork.Object, _userRepository.Object, _configuration.Object, _logger.Object);

        var result = await service.managesStore("notfound@example.com", Guid.NewGuid().ToString());

        Assert.False(result);
    }

    [Fact]
    public async Task ManagesStore_UserWithoutRole_ShouldReturnFalse()
    {
        var user = new User("NoRole", "912345678", "nobody@example.com", Configurations.HashString("pass", Salt), null, Salt);

        _userRepository.Setup(r => r.FindByEmail("nobody@example.com")).ReturnsAsync(user);

        var service = new AuthenticationService(_unitOfWork.Object, _userRepository.Object, _configuration.Object, _logger.Object);

        var result = await service.managesStore("nobody@example.com", Guid.NewGuid().ToString());

        Assert.False(result);
    }

    [Fact]
    public async Task ManagesStore_UserWithWrongRole_ShouldReturnFalse()
    {
        var user = new User("WrongRole", "912345678", "user@example.com", Configurations.HashString("pass", Salt), "client", Salt);
        var storeId = Guid.NewGuid().ToString();
        user.SetStore(storeId);

        _userRepository.Setup(r => r.FindByEmail("user@example.com")).ReturnsAsync(user);

        var service = new AuthenticationService(_unitOfWork.Object, _userRepository.Object, _configuration.Object, _logger.Object);

        var result = await service.managesStore("user@example.com", storeId);

        Assert.False(result);
    }

    [Fact]
    public async Task ManagesStore_UserWithRightRoleButDifferentStore_ShouldReturnFalse()
    {
        var user = new User("StoreAdmin", "912345678", "admin@example.com", Configurations.HashString("pass", Salt), Configurations.STORE_ADMIN_ROLE_NAME, Salt);
        user.SetStore(Guid.NewGuid().ToString());

        _userRepository.Setup(r => r.FindByEmail("admin@example.com")).ReturnsAsync(user);

        var service = new AuthenticationService(_unitOfWork.Object, _userRepository.Object, _configuration.Object, _logger.Object);

        var result = await service.managesStore("admin@example.com", Guid.NewGuid().ToString()); // Different storeId

        Assert.False(result);
    }

    [Fact]
    public async Task ManagesStore_UserWithRightRoleAndStore_ShouldReturnTrue()
    {
        var userEmail = "admin@example.com";
        var user = new User("StoreAdmin", "912345678", userEmail, Configurations.HashString("pass", Salt), Configurations.STORE_ADMIN_ROLE_NAME, Salt);

        var storeId = Guid.NewGuid().ToString();
        user.SetStore(storeId);

        _userRepository.Setup(r => r.FindByEmail(userEmail)).ReturnsAsync(user);

        var service = new AuthenticationService(_unitOfWork.Object, _userRepository.Object, _configuration.Object, _logger.Object);

        var result = await service.managesStore(userEmail, storeId);

        Assert.True(result);
    }

    [Fact]
    public async Task WorksOnStore_UserNotFound_ShouldReturnFalse()
    {
        _userRepository.Setup(r => r.FindByEmail("nouser@example.com"))
            .ReturnsAsync((User)null);

        var service = new AuthenticationService(_unitOfWork.Object, _userRepository.Object, _configuration.Object, _logger.Object);

        var result = await service.worksOnStore("nouser@example.com", Guid.NewGuid().ToString());

        Assert.False(result);
    }

    [Fact]
    public async Task WorksOnStore_UserWithoutRole_ShouldReturnFalse()
    {
        var user = new User("NoRole", "912345678", "nobody@example.com", Configurations.HashString("pass", Salt), null, Salt);
        _userRepository.Setup(r => r.FindByEmail("nobody@example.com")).ReturnsAsync(user);

        var service = new AuthenticationService(_unitOfWork.Object, _userRepository.Object, _configuration.Object, _logger.Object);

        var result = await service.worksOnStore("nobody@example.com", Guid.NewGuid().ToString());

        Assert.False(result);
    }

    [Fact]
    public async Task WorksOnStore_UserWithWrongRole_ShouldReturnFalse()
    {
        var user = new User("Client", "912345678", "client@example.com", Configurations.HashString("pass", Salt), Configurations.STORE_ADMIN_ROLE_NAME, Salt);
        user.SetStore(Guid.NewGuid().ToString());
        _userRepository.Setup(r => r.FindByEmail("client@example.com")).ReturnsAsync(user);

        var service = new AuthenticationService(_unitOfWork.Object, _userRepository.Object, _configuration.Object, _logger.Object);

        var result = await service.worksOnStore("client@example.com", user.Store!.AsString());

        Assert.False(result);
    }

    [Fact]
    public async Task WorksOnStore_UserWithRightRoleButDifferentStore_ShouldReturnFalse()
    {
        var userEmail = "colab@example.com";
        var user = new User("Colab", "912345678", userEmail, Configurations.HashString("pass", Salt), Configurations.STORE_COLAB_ROLE_NAME, Salt);
        user.SetStore(Guid.NewGuid().ToString());
        _userRepository.Setup(r => r.FindByEmail(userEmail)).ReturnsAsync(user);

        var service = new AuthenticationService(_unitOfWork.Object, _userRepository.Object, _configuration.Object, _logger.Object);

        var result = await service.worksOnStore(userEmail, Guid.NewGuid().ToString());

        Assert.False(result);
    }

    [Fact]
    public async Task ClientOnStore_UserNotFound_ShouldReturnFalse()
    {
        _userRepository.Setup(r => r.FindByEmail("nouser@example.com"))
            .ReturnsAsync((User)null);

        var service = new AuthenticationService(_unitOfWork.Object, _userRepository.Object, _configuration.Object, _logger.Object);

        var result = await service.clientOnStore("nouser@example.com", Guid.NewGuid().ToString());

        Assert.False(result);
    }

    [Fact]
    public async Task ClientOnStore_UserWithoutRole_ShouldReturnFalse()
    {
        var user = new User("NoRole", "912345678", "nobody@example.com", Configurations.HashString("pass", Salt), null, Salt);
        _userRepository.Setup(r => r.FindByEmail("nobody@example.com")).ReturnsAsync(user);

        var service = new AuthenticationService(_unitOfWork.Object, _userRepository.Object, _configuration.Object, _logger.Object);

        var result = await service.clientOnStore("nobody@example.com", Guid.NewGuid().ToString());

        Assert.False(result);
    }

    [Fact]
    public async Task ClientOnStore_UserWithWrongRole_ShouldReturnFalse()
    {
        var user = new User("Admin", "912345678", "admin@example.com", Configurations.HashString("pass", Salt), Configurations.STORE_ADMIN_ROLE_NAME, Salt);
        var storeId = Guid.NewGuid().ToString();
        user.SetStore(storeId);
        _userRepository.Setup(r => r.FindByEmail("admin@example.com")).ReturnsAsync(user);

        var service = new AuthenticationService(_unitOfWork.Object, _userRepository.Object, _configuration.Object, _logger.Object);

        var result = await service.clientOnStore("admin@example.com", storeId);

        Assert.False(result);
    }

    [Fact]
    public async Task ClientOnStore_UserWithRightRoleButDifferentStore_ShouldReturnFalse()
    {
        var userEmail = "user@example.com";
        var user = new User("User", "912345678", userEmail, Configurations.HashString("pass", Salt), Configurations.USER_ROLE_NAME, Salt);
        user.SetStore(Guid.NewGuid().ToString());
        _userRepository.Setup(r => r.FindByEmail(userEmail)).ReturnsAsync(user);

        var service = new AuthenticationService(_unitOfWork.Object, _userRepository.Object, _configuration.Object, _logger.Object);

        var result = await service.clientOnStore(userEmail, Guid.NewGuid().ToString());

        Assert.False(result);
    }
}