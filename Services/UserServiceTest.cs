using JetBrains.Annotations;
using Moq;
using ShopTex.Domain.Shared;
using ShopTex.Domain.Users;
using Xunit;

namespace ShopTex.Services;

[TestSubject(typeof(UserService))]
public class UserServiceTest
{
    private readonly Mock<IUnitOfWork> _unitOfWork = new Mock<IUnitOfWork>();
    private readonly Mock<IConfiguration> _configuration = new Mock<IConfiguration>();
    private readonly Mock<IUserRepository> _userRepository = new Mock<IUserRepository>();
    private readonly Mock<ILogger<UserService>> _logger = new Mock<ILogger<UserService>>();

    private static readonly byte[] Salt = UserService.GeneratePasswordSalt();
    private readonly User _testUser = new User("Testuser", "912345678", "testuser@example.com", UserService.HashString("passwd", Salt), UserRole.UserNRole.RoleName, "enabled", Salt);

    [Fact]
    public async Task UserSigninTest_Pass()
    {
        _userRepository.Setup(repository => repository.FindByEmail(It.IsAny<string>())).ReturnsAsync(_testUser);
        _configuration.Setup(configuration => configuration["Jwt:Key"]).Returns("testuserandineed128bitsforthiskeyoritdoesnotwork");

        var service = new UserService(_unitOfWork.Object, _userRepository.Object, _configuration.Object,_logger.Object);

        var result = await service.UserSignIn(new UserSignInDto()
            { Email = "testuser@example.com", Password = "passwd" });

        Assert.NotNull(result);
    }

    [Fact]
    public async Task UserSigninTest_Fail()
    {
        _userRepository.Setup(repository => repository.FindByEmail(It.IsAny<string>())).ReturnsAsync(_testUser);
        _configuration.Setup(configuration => configuration["Jwt:Key"]).Returns("testuserandineed128bitsforthiskeyoritdoesnotwork");

        var service = new UserService(_unitOfWork.Object, _userRepository.Object, _configuration.Object,_logger.Object);

        var result = await service.UserSignIn(new UserSignInDto() { Email = "testuser@example.com", Password = "passwdwrong" });

        Assert.Null(result);
    }
}
