using ShopTex.Domain.Shared;

namespace ShopTex.Domain.Users;

public interface IUserRepository : IRepository<User, UserId>
{
    public Task<User> FindByEmail(string email);

    Task<User> FindById(UserId userId);

    public void Update(User user);
}