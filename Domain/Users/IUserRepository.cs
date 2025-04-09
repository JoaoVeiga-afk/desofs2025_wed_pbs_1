using ShopTex.Domain.Shared;

namespace ShopTex.Domain.Users;

public interface IUserRepository : IRepository<User, UserId>
{
    public Task<User> FindByEmail(string email);
}