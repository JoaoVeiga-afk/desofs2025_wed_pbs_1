using ShopTex.Domain.Shared;
using ShopTex.Domain.Stores;
using ShopTex.Domain.Users;
using Microsoft.Extensions.Logging;


namespace ShopTex.Services;

public class StoreService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStoreRepository _repo;
    private readonly IUserRepository _userRepo;
    private readonly IConfiguration _config;
    private readonly ILogger<StoreService> _logger;

    public StoreService(IUnitOfWork unitOfWork, IStoreRepository repo, IUserRepository userRepo, IConfiguration config, ILogger<StoreService> logger)
    {
        _unitOfWork = unitOfWork;
        _repo = repo;
        _userRepo = userRepo;
        _config = config;
        _logger = logger;
    }

    public async Task<List<StoreDto>> GetAllAsync()
    {
        _logger.LogInformation("Fetching all stores started");
        var list = await _repo.GetAllAsync();
        _logger.LogInformation("Fetched {Count} stores from repository", list.Count);

        var dtoList = list.ConvertAll(store =>
        {
            _logger.LogDebug("Mapping store {StoreId} to StoreDto", store.Id.Value);
            return new StoreDto(store.Id.AsGuid(), store.Name, store.Address, store.Status);
        });

        _logger.LogInformation("Store mapping complete. Returning {Count} StoreDto objects", dtoList.Count);
        return dtoList;
    }

    public async Task<StoreDto?> GetByIdAsync(StoreId id)
    {
        _logger.LogInformation("Fetching store with ID {StoreId} started", id.Value);
        var store = await _repo.GetByIdAsync(id);

        if (store == null)
        {
            _logger.LogWarning("Store with ID {StoreId} not found", id.Value);
            return null;
        }

        _logger.LogInformation("Store with ID {StoreId} found. Name: {Name}", id.Value, store.Name);
        return new StoreDto(store.Id.AsGuid(), store.Name, store.Address, store.Status);
    }

    public async Task<StoreDto> AddAsync(CreatingStoreDto dto)
    {
        _logger.LogInformation("Creating new store with Name: {Name}, Address: {Address}, Status: {Status}",
            dto.Name, dto.Address, dto.Status);

        var store = new Store(dto.Name, dto.Address, dto.Status);

        try
        {
            await _repo.AddAsync(store);

            await _unitOfWork.CommitAsync();
        }
        catch
        {
            throw new BusinessRuleValidationException("Store already exists");
        }

        return new StoreDto(store.Id.AsGuid(), store.Name, store.Address, store.Status);
    }

    public async Task<(bool Success, string Message)> AddStoreColaborator(AddCollabDto dto)
    {
        _logger.LogInformation("Adding new collaborator with email {Email} to store with id {StoreId}", dto.UserEmail, dto.StoreId);

        // Find provided user
        var user = await _userRepo.FindByEmail(dto.UserEmail);
        if (user == null)
        {
            _logger.LogWarning("User with email {Email} not found", dto.UserEmail);
            return (false, "User not found.");
        }

        var storeExists = (await _repo.FindById(dto.StoreId)) != null;
        if (!storeExists)
        {
            _logger.LogWarning("Store with ID {StoreId} not found", dto.StoreId);
            return (false, "Store not found.");
        }

        var success = user.SetStore(dto.StoreId); // true or false

        if (!success)
        {
            _logger.LogWarning("User {Email} does not have a role that allows store assignment", dto.UserEmail);
            return (false, "User does not have the correct role for store assignment.");
        }

        _userRepo.Update(user); // Save changes
        _logger.LogInformation("User {Email} successfully assigned to store {StoreId}", dto.UserEmail, dto.StoreId);

        return (true, "Collaborator added to store successfully.");
    }
    
    public async Task<(bool Success, string Message)> AddStoreClient(AddClientDto dto, AuthenticatedUserDto userAuth)
    {
        User user;

        if (dto.UserId.HasValue)
        {
            var userId = new UserId(dto.UserId.Value); 

            user = await _userRepo.FindById(userId);
            if (user == null)
            {
                _logger.LogWarning("User with UserId {UserId} not found", dto.UserId.Value);
                return (false, "User not found.");
            }
        }
        else
        {
            user = await _userRepo.FindByEmail(userAuth.Email);
            if (user == null)
            {
                _logger.LogWarning("User with email {Email} not found", userAuth.Email);
                return (false, "User not found.");
            }
        }

        if (user.Store != null && !user.Store.Equals(default(StoreId)))  
        {
            _logger.LogWarning("User {Email} already has a store associated with StoreId {StoreId}", userAuth.Email, user.Store.Value);
            return (false, "User already has a store associated.");
        }

        var storeExists = (await _repo.FindById(dto.StoreId.ToString())) != null;
        if (!storeExists)
        {
            _logger.LogWarning("Store with ID {StoreId} not found", dto.StoreId);
            return (false, "Store not found.");
        }

        var success = user.SetClientStore(dto.StoreId.ToString());

        if (!success)
        {
            _logger.LogWarning("User {Email} does not have a role that allows store assignment", userAuth.Email);
            return (false, "User does not have the correct role for store assignment.");
        }

        _userRepo.Update(user);
        await _unitOfWork.CommitAsync();
        _logger.LogInformation("User {Email} successfully assigned to store {StoreId}", userAuth.Email, dto.StoreId);

        return (true, "Client added to store successfully.");
    }

}