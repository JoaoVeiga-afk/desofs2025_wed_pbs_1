﻿using ShopTex.Domain.Shared;
using ShopTex.Domain.Users;

namespace ShopTex.Services;

public class AuthenticationService(IUnitOfWork unitOfWork, IUserRepository repo, IConfiguration config, ILogger<UserService> logger)
{

    public async Task<bool> hasPermission(string email, List<UserRole> roles)
    {
        var user = await repo.FindByEmail(email);
        if (user == null)
        {
            logger.LogWarning("User with email {Email} not found", email);
            return false;
        }

        if (user.Role == null)
        {
            logger.LogWarning("User with email {Email} has no role assigned", email);
            return false;
        }

        if (roles.Contains(user.Role))
        {
            return true;
        }

        logger.LogWarning("User with email {Email} doesn't have permissions for this endpoint", email);
        return false;
    }

    public async Task<bool> managesStore(string email, string storeId)
    {
        var user = await repo.FindByEmail(email);
        if (user == null)
        {
            logger.LogWarning("User with email {Email} not found", email);
            return false;
        }

        if (user.Role == null)
        {
            logger.LogWarning("User with email {Email} has no role assigned", email);
            return false;
        }

        if (user.Role.RoleName != Configurations.STORE_ADMIN_ROLE_NAME)
        {
            logger.LogWarning("User with email {Email} doesn't have {Role} role", email, Configurations.STORE_ADMIN_ROLE_NAME);
            return false;
        }


        if (user.Store?.AsString() != storeId)
        {
            logger.LogWarning("User with email {Email} doesn't manage store with id {StoreId}", email, storeId);
            return false;
        }

        return true;
    }

    public async Task<bool> worksOnStore(string email, string storeId)
    {
        var user = await repo.FindByEmail(email);
        if (user == null)
        {
            logger.LogWarning("User with email {Email} not found", email);
            return false;
        }

        if (user.Role == null)
        {
            logger.LogWarning("User with email {Email} has no role assigned", email);
            return false;
        }

        if (user.Role.RoleName != Configurations.STORE_COLAB_ROLE_NAME)
        {
            logger.LogWarning("User with email {Email} doesn't have {Role} role", email, Configurations.STORE_COLAB_ROLE_NAME);
            return false;
        }

        if (user.Store?.AsString() != storeId)
        {
            logger.LogWarning("User with email {Email} doesn't work on store with id {StoreId}", email, storeId);

            return false;
        }

        return true;
    }

    public async Task<bool> clientOnStore(string email, string storeId)
    {
        var user = await repo.FindByEmail(email);
        if (user == null)
        {
            logger.LogWarning("User with email {Email} not found", email);
            return false;
        }

        if (user.Role == null)
        {
            logger.LogWarning("User with email {Email} has no role assigned", email);
            return false;
        }

        if (user.Role.RoleName != Configurations.USER_ROLE_NAME)
        {
            logger.LogWarning("User with email {Email} doesn't have {Role} role", email, Configurations.STORE_COLAB_ROLE_NAME);
            return false;
        }

        if (user.Store?.AsString() != storeId)
        {
            logger.LogWarning("User with email {Email} doesn't has account on store with id {StoreId}", email, storeId);

            return false;
        }

        return true;
    }
}