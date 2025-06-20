﻿using Microsoft.EntityFrameworkCore;
using ShopTex.Domain.Users;
using ShopTex.Infrastructure.Shared;
using ShopTex.Models;

namespace ShopTex.Infrastructure.Users;

public class UserRepository : BaseRepository<User, UserId>, IUserRepository
{
    public UserRepository(DatabaseContext context) : base(context.User)
    {

    }

    public async Task<User> FindByEmail(string email)
    {
        return await _objs.Where(u => email.Equals(u.Email.Value)).FirstOrDefaultAsync();
    }

    public async Task<User> FindById(UserId userId)  // Recebe UserId
    {
        return await _objs.Where(u => u.Id.Equals(userId)).FirstOrDefaultAsync(); // Comparando com UserId
    }
}