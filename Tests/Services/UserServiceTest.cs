using FluentAssertions;
using JetBrains.Annotations;
using Moq;
using ShopTex.Domain.Shared;
using ShopTex.Domain.Users;
using ShopTex.Services;
using ShopTex;
using Xunit;

namespace ShopTex.Tests.Services;

[TestSubject(typeof(UserService))]
public class UserServiceTest
{
    private readonly Mock<IUnitOfWork> _unitOfWork = new Mock<IUnitOfWork>();
    private readonly Mock<IConfiguration> _configuration = new Mock<IConfiguration>();
    private readonly Mock<IUserRepository> _userRepository = new Mock<IUserRepository>();
    private readonly Mock<ILogger<UserService>> _logger = new Mock<ILogger<UserService>>();

    private static readonly byte[] Salt = UserService.GeneratePasswordSalt();
    private readonly User _testUser = new User("Testuser", "912345678", "testuser@example.com", Configurations.HashString("passwd", Salt), UserRole.UserNRole.RoleName, Salt);

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

    [Fact]
    public async Task EnableUser_WhenUserIsDisabled_ShouldEnableUser_AndReturnUserDto()
    {
        // Arrange
        var id = new UserId(Guid.NewGuid());
        var disabledUser = new User(Guid.NewGuid().ToString(), "Testuser", "912345678", "testuser@example.com", "hash",
            UserRole.UserNRole.RoleName, "disabled", Salt);
        

        _userRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(disabledUser);

        var service = new UserService(_unitOfWork.Object, _userRepository.Object, _configuration.Object,
            _logger.Object);

        // Act
        var result = await service.EnableUser(id);

        // Assert
        result.Should().NotBeNull();
        result.Status.ToString().Should().Be("enabled");
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task EnableUser_WhenUserIsAlreadyEnabled_ShouldThrowException()
    {
        // Arrange
        var id = new UserId(Guid.NewGuid());
        var enabledUser = new User(Guid.NewGuid().ToString(),"Testuser", "912345678", "testuser@example.com", "hash", UserRole.UserNRole.RoleName,
            "enabled",Salt);

        _userRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(enabledUser);

        var service = new UserService(_unitOfWork.Object, _userRepository.Object, _configuration.Object,
            _logger.Object);

        // Act
        Func<Task> act = async () => await service.EnableUser(id);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleValidationException>()
            .WithMessage("User is already enabled");

        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task EnableUser_WhenUserDoesNotExist_ShouldThrowException()
    {
        // Arrange
        var id = new UserId(Guid.NewGuid());

        _userRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((User)null!);

        var service = new UserService(_unitOfWork.Object, _userRepository.Object, _configuration.Object,
            _logger.Object);

        // Act
        Func<Task> act = async () => await service.EnableUser(id);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleValidationException>()
            .WithMessage("User not found");

        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task DisableUser_WhenUserIsEnabled_ShouldDisableUser_AndReturnUserDto()
    {
        // Arrange
        var id = new UserId(Guid.NewGuid());
        var enabledUser = new User(Guid.NewGuid().ToString(),"Testuser", "912345678", "testuser@example.com", "hash", UserRole.UserNRole.RoleName,
            "enabled",Salt);

        _userRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(enabledUser);

        var service = new UserService(_unitOfWork.Object, _userRepository.Object, _configuration.Object,
            _logger.Object);

        // Act
        var result = await service.DisableUser(id);

        // Assert
        result.Should().NotBeNull();
        result.Status.ToString().Should().Be("disabled");
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task DisableUser_WhenUserIsAlreadyDisabled_ShouldThrowException()
    {
        // Arrange
        var id = new UserId(Guid.NewGuid());
        var disabledUser = new User(Guid.NewGuid().ToString(),"Testuser", "912345678", "testuser@example.com", "hash",
            UserRole.UserNRole.RoleName,"disabled", Salt);

        _userRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(disabledUser);

        var service = new UserService(_unitOfWork.Object, _userRepository.Object, _configuration.Object,
            _logger.Object);

        // Act
        Func<Task> act = async () => await service.DisableUser(id);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleValidationException>()
            .WithMessage("User is already disabled");

        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task DisableUser_WhenUserDoesNotExist_ShouldThrowException()
    {
        // Arrange
        var id = new UserId(Guid.NewGuid());

        _userRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((User)null!);

        var service = new UserService(_unitOfWork.Object, _userRepository.Object, _configuration.Object,
            _logger.Object);

        // Act
        Func<Task> act = async () => await service.DisableUser(id);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleValidationException>()
            .WithMessage("User not found");

        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }
}
