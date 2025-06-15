using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using ShopTex.Domain.Products;
using ShopTex.Domain.Shared;
using ShopTex.Domain.Stores;
using ShopTex.Domain.Users;
using ShopTex.Services;
using Xunit;

namespace ShopTex.Tests.Services
{
    public class ProductServiceTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWork = new();
        private readonly Mock<IProductRepository> _productRepo = new();
        private readonly Mock<IStoreRepository> _storeRepo = new();
        private readonly Mock<IConfiguration> _configuration = new();
        private readonly Mock<ILogger<ProductService>> _serviceLogger = new();
        private readonly Mock<IUserRepository> _userRepo = new();
        private readonly Mock<ILogger<UserService>> _userLogger = new();
        private readonly Mock<ILogger<UserService>> _authLogger = new();
        private readonly Mock<IHttpContextAccessor> _httpContextAccessor = new();

        private readonly AuthenticationService _authService;
        private readonly UserService _userService;
        private readonly ProductService _service;
        private const string TestImageStoragePath = "TestProductImages";

        public ProductServiceTest()
        {
            _userService = new UserService(
                _unitOfWork.Object,
                _userRepo.Object,
                _configuration.Object,
                _userLogger.Object
            );

            _authService = new AuthenticationService(
                _unitOfWork.Object,
                _userRepo.Object,
                _configuration.Object,
                _authLogger.Object
            );

            _service = new ProductService(
                _unitOfWork.Object,
                _productRepo.Object,
                _storeRepo.Object,
                _configuration.Object,
                _serviceLogger.Object,
                _authService,
                _userService,
                _httpContextAccessor.Object,
                TestImageStoragePath
            );
        }

        private static Product CreateTestProduct(string storeId)
            => new Product("Test Product", "Test Desc", 9.99, "Category A", "enabled", storeId);

        private static AuthenticatedUserDto CreateAuth(string email = "test@domain.com")
            => new() { Email = email };

        [Fact]
        public async Task GetAllProductsAsync_AsSysAdmin_ReturnsAllProducts()
        {
            var userAuth = CreateAuth();
            var fakeUser = new User(
                name: "Admin",
                phone: "000",
                email: userAuth.Email,
                password: "x",
                role: "System Administrator",
                salt: Array.Empty<byte>()
            );
            _userRepo
                .Setup(r => r.FindByEmail(userAuth.Email))
                .ReturnsAsync(fakeUser);

            var product = CreateTestProduct(Guid.NewGuid().ToString());
            _productRepo
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Product> { product });

            var result = await _service.GetAllProductsAsync(userAuth);

            Assert.Single(result);
            Assert.Equal(product.Name, result[0].Name);
        }

        [Fact]
        public async Task GetAllProductsAsync_AsStoreUser_FiltersByStore()
        {
            var userAuth = CreateAuth();
            var fakeUser = new User(
                name: "StoreUser",
                phone: "111",
                email: userAuth.Email,
                password: "x",
                role: "Store Collaborator",
                salt: Array.Empty<byte>()
            );
            var storeId = Guid.NewGuid();
            var storeProp = typeof(User)
                .GetProperty("Store", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)!;
            storeProp.SetValue(fakeUser, new StoreId(storeId));

            _userRepo
                .Setup(r => r.FindByEmail(userAuth.Email))
                .ReturnsAsync(fakeUser);

            var product1 = CreateTestProduct(storeId.ToString());
            var product2 = CreateTestProduct(Guid.NewGuid().ToString());
            _productRepo
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Product> { product1, product2 });

            var result = await _service.GetAllProductsAsync(userAuth);

            Assert.Single(result);
            Assert.Equal(storeId.ToString(), result[0].StoreId);
        }

        [Fact]
        public async Task GetProductByIdAsync_AsSysAdmin_ReturnsProductDto()
        {
            var userAuth = CreateAuth();
            var fakeUser = new User(
                name: "Admin",
                phone: "000",
                email: userAuth.Email,
                password: "x",
                role: "System Administrator",
                salt: Array.Empty<byte>()
            );
            _userRepo
                .Setup(r => r.FindByEmail(userAuth.Email))
                .ReturnsAsync(fakeUser);

            var id = new ProductId(Guid.NewGuid());
            var product = CreateTestProduct(Guid.NewGuid().ToString());
            _productRepo
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync(product);

            var result = await _service.GetProductByIdAsync(id, userAuth);

            Assert.NotNull(result);
            Assert.Equal(product.Name, result?.Name);
        }

        [Fact]
        public async Task GetProductByIdAsync_NotFound_ReturnsNull()
        {
            var userAuth = CreateAuth();
            var fakeUser = new User(
                name: "Admin",
                phone: "000",
                email: userAuth.Email,
                password: "x",
                role: "System Administrator",
                salt: Array.Empty<byte>()
            );
            _userRepo
                .Setup(r => r.FindByEmail(userAuth.Email))
                .ReturnsAsync(fakeUser);

            var id = new ProductId(Guid.NewGuid());
            _productRepo
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync((Product?)null);

            var result = await _service.GetProductByIdAsync(id, userAuth);

            Assert.Null(result);
        }

        [Fact]
        public async Task AddAsync_StoreNotFound_Throws()
        {
            var dto = new CreatingProductDto(
                id: "1",
                name: "Name",
                description: "Desc",
                price: 1.0,
                category: "Cat",
                status: new ProductStatus("enabled"),
                storeId: new StoreId(Guid.NewGuid())
            );
            _storeRepo
                .Setup(r => r.FindById(dto.StoreId))
                .ReturnsAsync((Store?)null);

            await Assert.ThrowsAsync<BusinessRuleValidationException>(
                () => _service.AddAsync(dto)
            );
        }

        [Fact]
        public async Task AddAsync_Valid_ReturnsDto()
        {
            var storeId = new StoreId(Guid.NewGuid());
            var store = new Store("S", new StoreAddress("St", "C", "ST", "12345", "CL"), "enabled");
            var dto = new CreatingProductDto(
                id: "1",
                name: "N",
                description: "D",
                price: 2.0,
                category: "C",
                status: new ProductStatus("enabled"),
                storeId: storeId
            );
            _storeRepo
                .Setup(r => r.FindById(storeId.AsString()))
                .ReturnsAsync(store);
            _productRepo
                .Setup(r => r.AddAsync(It.IsAny<Product>()))
                .ReturnsAsync((Product p) => p);
            _unitOfWork
                .Setup(u => u.CommitAsync())
                .ReturnsAsync(1);

            var result = await _service.AddAsync(dto);

            Assert.Equal(dto.Name, result.Name);
            Assert.Equal(dto.StoreId, result.StoreId);
        }

        [Fact]
        public async Task UploadImage_ProductNotFound_Throws()
        {
            var id = Guid.NewGuid().ToString();
            _productRepo
                .Setup(r => r.FindById(id))
                .ReturnsAsync((Product?)null);

            await Assert.ThrowsAsync<BusinessRuleValidationException>(
                () => _service.UploadImage(id, new byte[] { 1 })
            );
        }
    }
}
