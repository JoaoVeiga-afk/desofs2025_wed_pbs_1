using ShopTex.Domain.Shared;
using ShopTex.Domain.Products;
using ShopTex.Domain.Stores;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ShopTex.Services;

public class ProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductRepository _repo;
    private readonly IStoreRepository _storeRepo;
    private readonly IConfiguration _config;
    private readonly ILogger<ProductService> _logger;
    private readonly string _imageStoragePath;

    public ProductService(IUnitOfWork unitOfWork, IProductRepository repo, IStoreRepository storeRepo, IConfiguration config, ILogger<ProductService> logger, string imageStoragePath = Configurations.IMAGE_STORAGE_PATH)
    {
        _unitOfWork = unitOfWork;
        _repo = repo;
        _storeRepo = storeRepo;
        _config = config;
        _logger = logger;
        _imageStoragePath = imageStoragePath;
    }

    public async Task<List<ProductDto>> GetAllAsync()
    {
        _logger.LogInformation("Fetching all products started");
        var list = await _repo.GetAllAsync();
        _logger.LogInformation("Fetched {Count} products from repository", list.Count);

        var dtoList = list.ConvertAll(product =>
        {
            _logger.LogDebug("Mapping product {ProductId} to ProductDto", product.Id.Value);
            return new ProductDto(product.Id.AsString(), product.Name, product.Description, product.Price, product.Category, product.Status, product.StoreId);
        });

        _logger.LogInformation("Product mapping complete. Returning {Count} ProductDto objects", dtoList.Count);
        return dtoList;
    }

    public async Task<ProductDto?> GetByIdAsync(ProductId id)
    {
        _logger.LogInformation("Fetching product with ID {ProductId} started", id.Value);
        var product = await _repo.FindById(id.AsString());

        if (product == null)
        {
            _logger.LogWarning("Product with ID {ProductId} not found", id.Value);
            return null;
        }

        _logger.LogInformation("Product with ID {ProductId} found. Name: {Name}", id.Value, product.Name);
        return new ProductDto(product.Id.AsString(), product.Name, product.Description, product.Price, product.Category, product.Status, product.StoreId);
    }

    public async Task<ProductDto> AddAsync(ProductDto dto)
    {
        _logger.LogInformation("Creating new product with Name: {Name}, Description: {Description}, Price: {Price}, Category: {Category}, Status: {Status}, Store Id: {StoreId}",
            dto.Name, dto.Description, dto.Price, dto.Category, dto.Status, dto.StoreId);

        if ((await _storeRepo.FindById(dto.StoreId)) == null)
        {
            throw new BusinessRuleValidationException("Store Id does not exist");
        }

        var product = new Product(dto.Name, dto.Description, dto.Price, dto.Category, dto.Status, dto.StoreId);

        try
        {
            await _repo.AddAsync(product);

            await _unitOfWork.CommitAsync();
        }
        catch
        {
            throw new BusinessRuleValidationException("Product already exists");
        }

        return new ProductDto(product.Id.AsString(), product.Name, product.Description, product.Price, product.Category, product.Status, product.StoreId);
    }

    public async Task<bool> UploadImage(string productId, byte[] image)
    {
        var product = await _repo.FindById(productId);

        if (product == null)
            throw new BusinessRuleValidationException("Product not found");

        var success = product.UploadImage(image, _imageStoragePath);
        if (!success)
            return false;

        await _unitOfWork.CommitAsync(); // Make sure changes are saved
        return true;
    }
}