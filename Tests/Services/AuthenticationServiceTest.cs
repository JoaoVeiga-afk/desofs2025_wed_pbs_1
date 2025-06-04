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
    
    
}