using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using ShopTex.Domain.Shared;
using ShopTex.Domain.Users;
using Microsoft.Extensions.Logging;

namespace ShopTex.Services;

public class UserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _repo;
    private readonly IConfiguration _config;
    private readonly ILogger<UserService> _logger;

    public UserService(IUnitOfWork unitOfWork, IUserRepository repo, IConfiguration config, ILogger<UserService> logger)
    {
        _unitOfWork = unitOfWork;
        _repo = repo;
        _config = config;
        _logger = logger;
    }

    public async Task<List<UserDto>> GetAllAsync()
    {
        _logger.LogInformation("Fetching all users started");
        var list = await _repo.GetAllAsync();
        _logger.LogInformation("Fetched {Count} users from repository", list.Count);

        var dtoList = list.ConvertAll(user =>
        {
            _logger.LogDebug("Mapping user {UserId} to UserDto", user.Id.Value);
            return new UserDto(user.Id.AsGuid(), user.Name, user.Phone, user.Email ,user.Role, user.Status);
        });

        _logger.LogInformation("User mapping complete. Returning {Count} UserDto objects", dtoList.Count);
        return dtoList;
    }

    public async Task<UserDto?> GetByIdAsync(UserId id)
    {
        _logger.LogInformation("Fetching user with ID {UserId} started", id.Value);
        var user = await _repo.GetByIdAsync(id);

        if (user == null)
        {
            _logger.LogWarning("User with ID {UserId} not found", id.Value);
            return null;
        }

        _logger.LogInformation("User with ID {UserId} found. Name: {Name}, Email: {Email}", id.Value, user.Name, user.Email);
        return new UserDto(user.Id.AsGuid(), user.Name, user.Phone, user.Email, user.Role, user.Status);
    }

    public async Task<UserDto> AddAsync(CreatingUserDto dto)
    {
        _logger.LogInformation("Creating new user with Name: {Name}, Email: {Email}, Role: {RoleId}",
            dto.Name, dto.Email, dto.RoleId);

        var salt = GeneratePasswordSalt();

        var hashPassword = Configurations.HashString(dto.Password, salt);

        var user = new User(dto.Name, dto.Phone, dto.Email, hashPassword, dto.RoleId, salt);

        try
        {
            await _repo.AddAsync(user);

            await _unitOfWork.CommitAsync();
        }
        catch (Exception ex)
        {
            throw new BusinessRuleValidationException("User already exists with this email");
        }

        return new UserDto(user.Id.AsGuid(), user.Name, user.Phone, user.Email, user.Role, user.Status);
    }

    public async Task<dynamic?> UserSignIn(UserSignInDto dto)
    {
        _logger.LogInformation("User login attempt for Email {Email}", dto.Email);

        var user = await _repo.FindByEmail(dto.Email);

        if (user == null)
        {
            _logger.LogWarning("Login failed: User with Email {Email} not found", dto.Email);
            return null;
        }

        if (!user.Status.Value)
        {
            _logger.LogWarning("Login failed: User with Email {Email} is inactive", dto.Email);
            return null;
        }

        if (!user.VerifyPassword(dto.Password))
        {
            _logger.LogWarning("Login failed: Incorrect password for Email {Email}", dto.Email);
            return null;
        }

        _logger.LogInformation("User with Email {Email} authenticated successfully. UserId: {UserId}", dto.Email, user.Id.Value);

        var token = GenerateToken(user);

        return new
        {
            userDTO = new UserDto(user.Id.AsGuid(), user.Name, user.Phone, user.Email, user.Role, user.Status),
            token
        };
    }

    private string GenerateToken(User user)
    {
        _logger.LogInformation("Generating JWT token for UserId {UserId} with Role {RoleName}", user.Id.Value, user.Role?.RoleName ?? "none");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("guid", user.Id.Value),
            new Claim("name", user.Name),
            new Claim("email", user.Email.Value),
            new Claim("role", user.Role?.RoleName ?? "none"),
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Issuer"],
            claims: claims,
            expires: DateTime.Now.AddDays(30),
            signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        _logger.LogDebug("Token string length: {Length}", tokenString.Length);

        return tokenString;
    }

    public static byte[] GeneratePasswordSalt()
    {
        byte[] salt = new byte[32];
        RandomNumberGenerator.Create().GetBytes(salt);
        return salt;
    }
    
}
