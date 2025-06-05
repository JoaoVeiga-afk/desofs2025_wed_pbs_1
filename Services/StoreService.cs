using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using ShopTex.Domain.Shared;
using ShopTex.Domain.Stores;
using Microsoft.Extensions.Logging;

namespace ShopTex.Services;

public class StoreService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStoreRepository _repo;
    private readonly IConfiguration _config;
    private readonly ILogger<StoreService> _logger;

    public StoreService(IUnitOfWork unitOfWork, IStoreRepository repo, IConfiguration config, ILogger<StoreService> logger)
    {
        _unitOfWork = unitOfWork;
        _repo = repo;
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
        catch (Exception ex)
        {
            throw new BusinessRuleValidationException("Store already exists");
        }

        return new StoreDto(store.Id.AsGuid(), store.Name, store.Address, store.Status);
    }
}