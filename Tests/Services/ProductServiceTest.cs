using Moq;
using ShopTex.Domain.Products;
using ShopTex.Domain.Shared;
using ShopTex.Domain.Stores;
using ShopTex.Services;
using Xunit;

namespace ShopTex.Tests.Services;

public class ProductServiceTest
{
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IProductRepository> _productRepository = new();
    private readonly Mock<IStoreRepository> _storeRepository = new();
    private readonly Mock<IConfiguration> _configuration = new();
    private readonly Mock<ILogger<ProductService>> _logger = new();
    private const string TestImageStoragePath = "TestProductImages";

    private ProductService CreateService()
    {
        return new ProductService(_unitOfWork.Object, _productRepository.Object, _storeRepository.Object, _configuration.Object, _logger.Object, TestImageStoragePath);
    }

    private static Product CreateTestProduct(string storeId)
    {
        return new Product("Test Product", "Test Desc", 9.99, "Category A", "enabled", storeId);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsListOfProductDtos()
    {
        // Arrange
        var product = CreateTestProduct(Guid.NewGuid().ToString());
        _productRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Product> { product });

        var service = CreateService();

        // Act
        var result = await service.GetAllAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal("Test Product", result[0].Name);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingProduct_ReturnsProductDto()
    {
        // Arrange
        var productId = new ProductId(Guid.NewGuid());
        var product = CreateTestProduct(Guid.NewGuid().ToString());
        Assert.NotNull(product);

        _productRepository.Setup(r => r.FindById(productId.AsString())).ReturnsAsync(product);

        var service = CreateService();

        // Act
        var result = await service.GetByIdAsync(productId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Product", result?.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ProductNotFound_ReturnsNull()
    {
        var productId = Guid.NewGuid();
        _productRepository.Setup(r => r.FindById(productId.ToString())).ReturnsAsync((Product?)null);

        var service = CreateService();

        var result = await service.GetByIdAsync(new ProductId(productId));

        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_StoreNotFound_ThrowsBusinessRuleValidationException()
    {
        // Arrange
        var storeId = new StoreId(Guid.NewGuid());
        var dto = new ProductDto("1", "New Product", "New Desc", 10.00, "Category", new ProductStatus("enabled"), storeId);

        _storeRepository.Setup(r => r.FindById(storeId.AsString())).ReturnsAsync((Store?)null);

        var service = CreateService();

        // Act & Assert
        await Assert.ThrowsAsync<BusinessRuleValidationException>(() => service.AddAsync(dto));
    }

    [Fact]
    public async Task AddAsync_ValidProduct_ReturnsProductDto()
    {
        // Arrange
        var storeId = new StoreId(Guid.NewGuid());
        var store = new Store("Test Store", new StoreAddress("St", "C", "ST", "12345", "CL"), "enabled");

        var dto = new ProductDto("1", "Product A", "Desc A", 15.5, "Cat A", new ProductStatus("enabled"), storeId);

        _storeRepository.Setup(r => r.FindById(storeId.AsString())).ReturnsAsync(store);
        _productRepository.Setup(r => r.AddAsync(It.IsAny<Product>())).ReturnsAsync(new Product(dto.Name, dto.Description, dto.Price, dto.Category, dto.Status, dto.StoreId));
        _unitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(1);

        var service = CreateService();

        // Act
        var result = await service.AddAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dto.Name, result.Name);
        Assert.Equal(dto.StoreId, result.StoreId);
    }

    [Fact]
    public async Task AddAsync_AddFails_ThrowsBusinessRuleValidationException()
    {
        // Arrange
        var storeId = new StoreId(Guid.NewGuid());
        var store = new Store("Test Store", new StoreAddress("St", "C", "ST", "12345", "CL"), "enabled");
        var dto = new ProductDto("1", "Duplicate", "Desc", 20.0, "Cat", new ProductStatus("enabled"), storeId);

        _storeRepository.Setup(r => r.FindById(storeId.AsString())).ReturnsAsync(store);
        _productRepository.Setup(r => r.AddAsync(It.IsAny<Product>())).ThrowsAsync(new Exception("Insert failed"));

        var service = CreateService();

        // Act & Assert
        await Assert.ThrowsAsync<BusinessRuleValidationException>(() => service.AddAsync(dto));
    }

    [Fact]
    public async Task UploadImage_ProductNotFound_ThrowsBusinessRuleValidationException()
    {
        // Arrange
        var productId = Guid.NewGuid().ToString();
        _productRepository.Setup(r => r.FindById(productId)).ReturnsAsync((Product?)null);

        var service = CreateService();

        // Act & Assert
        await Assert.ThrowsAsync<BusinessRuleValidationException>(() => service.UploadImage(productId, new byte[] { 1, 2, 3 }));
    }

    [Fact]
    public async Task UpdateAsync_ValidProduct_ReturnsProductDto()
    {
        var storeId = new StoreId(Guid.NewGuid());
        var store = new Store(storeId.Value, "Test Store", new StoreAddress("St", "C", "ST", "12345", "CL"), "enabled");
        var productId = new ProductId(Guid.NewGuid());
        var product = CreateTestProduct(storeId.Value);
        var productDto = new ProductDto(productId.Value, product.Name, product.Description, product.Price, product.Category, product.Status, product.StoreId);
        _productRepository.Setup(r => r.FindById(productId.AsString())).ReturnsAsync(product);
        _storeRepository.Setup(r => r.FindById(storeId.Value)).ReturnsAsync(store);
        var service = CreateService();

        Assert.Equal(await service.UpdateAsync(productDto), productDto);
    }
}
