using UserManager.Domain.Shared;

namespace UserManager.Domain.Users;

public interface IUserRepository : IRepository<User, UserId>
{
    public Task<User> FindByEmail(string email);
}