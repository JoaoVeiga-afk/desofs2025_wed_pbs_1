using ShopTex.Domain.Shared;
using ShopTex.Domain.Products;
using ShopTex.Domain.Stores;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using ShopTex.Domain.Users;

namespace ShopTex.Services;

public class ProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductRepository _repo;
    private readonly IStoreRepository _storeRepo;
    private readonly IConfiguration _config;
    private readonly ILogger<ProductService> _logger;
    private readonly AuthenticationService _authenticationService;
    private readonly UserService _userService;
    private readonly string _imageStoragePath;

    public ProductService(
        IUnitOfWork unitOfWork,
        IProductRepository repo,
        IStoreRepository storeRepo,
        IConfiguration config,
        ILogger<ProductService> logger,
        AuthenticationService authenticationService,
        UserService userService,
        string imageStoragePath = Configurations.IMAGE_STORAGE_PATH 
    )
    {
        _unitOfWork = unitOfWork;
        _repo = repo;
        _storeRepo = storeRepo;
        _config = config;
        _logger = logger;
        _authenticationService = authenticationService;
        _userService = userService;
        _imageStoragePath = imageStoragePath;
    }

   public async Task<List<ProductDto>> GetAllProductsAsync(AuthenticatedUserDto userAuth)
    {
        _logger.LogInformation("Validating access for {Email}", userAuth.Email);

        var isSysAdmin = await _authenticationService
            .hasPermission(userAuth.Email, new List<UserRole>{ UserRole.SystemRole });

        string? userStoreId = null;
        if (!isSysAdmin)
        {
            var user = await _userService.GetUserByEmailAsync(userAuth.Email)
                       ?? throw new BusinessRuleValidationException("Authenticated user not found.");

            userStoreId = user.Store?.AsGuid().ToString()
                          ?? throw new UnauthorizedAccessException("You don't have a store associated with this user.");
            
            _logger.LogInformation("User {Email} is not sysadmin – will only see products from store {StoreId}", userAuth.Email, userStoreId);
        }
        else
        {
            _logger.LogInformation("User {Email} is sysadmin – will see all products", userAuth.Email);
        }

        _logger.LogInformation("Fetching all products started");
        var list = await _repo.GetAllAsync();
        _logger.LogInformation("Fetched {Count} products from repository", list.Count);

        if (!isSysAdmin)
        {
            list = list.Where(p => p.StoreId.AsString() == userStoreId).ToList();
            _logger.LogInformation("Filtered down to {Count} products for store {StoreId}", list.Count, userStoreId);
        }

        var dtoList = list.ConvertAll(product =>
        {
            _logger.LogDebug("Mapping product {ProductId} to ProductDto", product.Id.Value);
            return new ProductDto(
                product.Id.AsString(),
                product.Name,
                product.Description,
                product.Price,
                product.Category,
                product.Status,
                product.StoreId);
        });

        _logger.LogInformation("Product mapping complete. Returning {Count} ProductDto objects", dtoList.Count);
        return dtoList;
    }


    public async Task<ProductDto?> GetProductByIdAsync(ProductId id, AuthenticatedUserDto userAuth)
    {
        _logger.LogInformation("Validating access for {Email} to product {ProductId}", userAuth.Email, id.Value);

        var isSysAdmin = await _authenticationService
            .hasPermission(userAuth.Email, new List<UserRole> { UserRole.SystemRole });

        string? userStoreId = null;
        if (!isSysAdmin)
        {
            var user = await _userService.GetUserByEmailAsync(userAuth.Email)
                       ?? throw new BusinessRuleValidationException("Authenticated user not found.");

            userStoreId = user.Store?.AsGuid().ToString()
                          ?? throw new UnauthorizedAccessException("You don't have a store associated with this user.");

            _logger.LogInformation(
                "User {Email} is not sysadmin – will only access products from store {StoreId}",
                userAuth.Email, userStoreId);
        }
        else
        {
            _logger.LogInformation("User {Email} is sysadmin – may access any product", userAuth.Email);
        }

        _logger.LogInformation("Fetching product {ProductId}", id.Value);
        var product = await _repo.GetByIdAsync(id);
        if (product == null)
        {
            _logger.LogWarning("Product {ProductId} not found", id.Value);
            return null;
        }

        if (!isSysAdmin && product.StoreId.AsString() != userStoreId)
        {
            _logger.LogWarning(
                "User {Email} attempted to access product {ProductId} from store {ProductStore} – access denied",
                userAuth.Email, id.Value, product.StoreId.AsString());
            throw new UnauthorizedAccessException("No products available.");
        }

        _logger.LogInformation("Access granted to product {ProductId}", id.Value);
        return new ProductDto(
            product.Id.AsString(),
            product.Name,
            product.Description,
            product.Price,
            product.Category,
            product.Status,
            product.StoreId);
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
    
    private async Task ValidateUserAccessAsync(AuthenticatedUserDto userAuth)
    {
        var user = await _userService.GetUserByEmailAsync(userAuth.Email)
                   ?? throw new BusinessRuleValidationException("Authenticated user not found.");

        string? storeId = user.Store?.AsGuid().ToString();

        if (string.IsNullOrWhiteSpace(storeId))
        {
            throw new UnauthorizedAccessException("You don't have a store associated with this user.");
        }

        var hasPermission = await UserCanAccessOrderAsync(userAuth.Email, storeId);

        if (!hasPermission)
        {
            throw new UnauthorizedAccessException("You don't have permission");
        }
    }
    
    private async Task<bool> UserCanAccessOrderAsync(string email, String storeId)
    {
        var sysAdmin = await _authenticationService.hasPermission(email, new List<UserRole> { UserRole.SystemRole });
        var storeAdmin = await _authenticationService.managesStore(email, storeId);
        var storeColab = await _authenticationService.worksOnStore(email, storeId);
        var clientStore = await _authenticationService.clientOnStore(email, storeId);

        return sysAdmin || storeAdmin || storeColab || clientStore;
    }

}