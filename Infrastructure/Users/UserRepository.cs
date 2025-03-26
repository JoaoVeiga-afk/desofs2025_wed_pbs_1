using Microsoft.EntityFrameworkCore;
using UserManager.Domain.Users;
using UserManager.Infrastructure.Shared;
using UserManager.Models;

namespace UserManager.Infrastructure.Users;

public class UserRepository : BaseRepository<User,UserId>,IUserRepository
{
    public UserRepository(DatabaseContext context) : base(context.Users)
    {
        
    }

    public async Task<User> FindByEmail(string email)
    {
        return await _objs.Where(u => email.Equals(u.Email.Value)).FirstOrDefaultAsync();
    }
}