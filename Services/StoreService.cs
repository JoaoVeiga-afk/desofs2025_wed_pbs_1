using ShopTex.Domain.Shared;
using ShopTex.Domain.Stores;
using ShopTex.Domain.Users;

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

    public async Task<(bool Success, string Message)> AddStoreColaborator(string storeId, string userEmail)
    {
        _logger.LogInformation("Adding new collaborator with email {Email} to store with id {StoreId}", userEmail, storeId);

        // Find provided user
        var user = await _userRepo.FindByEmail(userEmail);
        if (user == null)
        {
            _logger.LogWarning("User with email {Email} not found", userEmail);
            return (false, "User not found.");
        }

        var storeExists = (await _repo.FindById(storeId)) != null;
        if (!storeExists)
        {
            _logger.LogWarning("Store with ID {StoreId} not found", storeId);
            return (false, "Store not found.");
        }

        var success = user.SetStore(storeId); // true or false

        if (!success)
        {
            _logger.LogWarning("User {Email} does not have a role that allows store assignment", userEmail);
            return (false, "User does not have the correct role for store assignment.");
        }

        _userRepo.Update(user); // Save changes
        _logger.LogInformation("User {Email} successfully assigned to store {StoreId}", userEmail, storeId);

        return (true, "Collaborator added to store successfully.");
    }
}