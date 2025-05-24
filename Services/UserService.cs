using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using ShopTex.Domain.Shared;
using ShopTex.Domain.Users;

namespace ShopTex.Services;

public class UserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _repo;
    private readonly IConfiguration _config;

    public UserService(IUnitOfWork unitOfWork, IUserRepository repo, IConfiguration config)
    {
        _unitOfWork = unitOfWork;
        _repo = repo;
        _config = config;
    }

    public async Task<List<UserDto>> GetAllAsync()
    {
        var list = await _repo.GetAllAsync();

        List<UserDto> listDto = list.ConvertAll(user =>
            new UserDto(user.Id.AsGuid(), user.FirstName, user.LastName, user.Phone, user.Email, user.Role, user.Nif, user.Status));

        return listDto;
    }

    public async Task<UserDto> GetByIdAsync(UserId id)
    {
        var user = await _repo.GetByIdAsync(id);

        if (user == null) return null;

        return new UserDto(user.Id.AsGuid(), user.FirstName, user.LastName, user.Phone, user.Email, user.Role, user.Nif, user.Status);
    }

    public async Task<UserDto> AddAsync(CreatingUserDto dto)
    {
        var salt = GeneratePasswordSalt();

        string hashPassword = HashString(dto.Password, salt);

        User user;
        
        if (dto.Id == null)
            user = new User(dto.FirstName, dto.LastName, dto.Phone, dto.Email, hashPassword, dto.Role, dto.Nif, dto.Status, salt);
        else
            user = new User(dto.Id, dto.FirstName, dto.LastName, dto.Phone, dto.Email, hashPassword, dto.Role, dto.Nif, dto.Status, salt);
        
        await _repo.AddAsync(user);

        await _unitOfWork.CommitAsync();

        return new UserDto(user.Id.AsGuid(), user.FirstName, user.LastName, user.Phone, user.Email, user.Role, user.Nif, user.Status);
    }
    public async Task<dynamic> UserSignIn(UserSignInDto dto)
    {
        var user = await _repo.FindByEmail(dto.Email);

        if (user == null) return null;

        if (!user.Status.Value) return null;

        if (!VerifyPassword(dto.Password, user.Salt, user.Password))
        {
            return null;
             
        }
        var result = new
        {
            userDTO = new UserDto(user.Id.AsGuid(), user.FirstName, user.LastName, user.Phone, user.Email, user.Role, user.Nif, user.Status),
            token = GenerateToken(user)
        };
        
        
        return result;

    }

    private string GenerateToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("guid", user.Id.Value),
            new Claim("firstName", user.FirstName),
            new Claim("lastName", user.LastName),
            new Claim("email", user.Email.Value),
            new Claim("role", user.Role != null ? user.Role.RoleName : "none"),
        };

        var token = new JwtSecurityToken(_config["Jwt:Issuer"], _config["Jwt:Issuer"], claims,
            expires: DateTime.Now.AddDays(30), signingCredentials: cred);
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    private static byte[] GeneratePasswordSalt()
    {
        byte[] salt = new byte[32];
        var rng = RandomNumberGenerator.Create();
        rng.GetBytes(salt);
        
        return salt;
    }

    private static string HashString(string source, byte[] salt)
    {
        var hasher = new Argon2id(Encoding.UTF8.GetBytes(source));
        hasher.Salt = salt;
        hasher.Iterations = 2;
        hasher.MemorySize = 1024;
        hasher.DegreeOfParallelism = 1;
        byte[] hashBytes = hasher.GetBytes(32);
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }

    private static bool VerifyPassword(string password, byte[] salt, string hashedPassword)
    {
        return hashedPassword.Equals(HashString(password, salt));
    }
    
}