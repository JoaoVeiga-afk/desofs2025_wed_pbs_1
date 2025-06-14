using FluentAssertions;
using Moq;
using ShopTex.Domain.Orders;
using ShopTex.Domain.OrdersProduct;
using ShopTex.Domain.Products;
using ShopTex.Domain.Shared;
using ShopTex.Domain.Stores;
using ShopTex.Domain.Users;
using ShopTex.Services;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace ShopTex.Tests.Services
{
    public class OrderServiceTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWork = new();
        private readonly Mock<IOrderRepository> _orderRepo = new();
        private readonly Mock<IOrderProductRepository> _orderProductRepo = new();
        private readonly Mock<IProductRepository> _productRepo = new();
        private readonly Mock<ILogger<OrderService>> _orderLogger = new();
        private readonly Mock<IUserRepository> _userRepo = new();
        private readonly Mock<IConfiguration> _configuration = new();
        private readonly Mock<ILogger<UserService>> _userLogger = new();
        private readonly Mock<ILogger<UserService>> _authLogger = new();

        private readonly AuthenticationService _authService;
        private readonly OrderService _service;

        public OrderServiceTest()
        {
            var fakeUser = new User(
                name: "Teste",
                phone: "999999999",
                email: "test@example.com",
                password: "irrelevant",
                role: "System Administrator",
                salt: Array.Empty<byte>()
            );

            var storeProp = typeof(User)
                .GetProperty("Store", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            storeProp!.SetValue(fakeUser, new StoreId(Guid.NewGuid()));

            _userRepo
                .Setup(r => r.FindByEmail(It.IsAny<string>()))
                .ReturnsAsync(fakeUser);
            _userRepo
                .Setup(r => r.GetByIdAsync(It.IsAny<UserId>()))
                .ReturnsAsync(fakeUser);

            _authService = new AuthenticationService(
                _unitOfWork.Object,
                _userRepo.Object,
                _configuration.Object,
                _authLogger.Object
            );

            var userService = new UserService(
                _unitOfWork.Object,
                _userRepo.Object,
                _configuration.Object,
                _userLogger.Object
            );

            _service = new OrderService(
                _unitOfWork.Object,
                _orderRepo.Object,
                _orderProductRepo.Object,
                _productRepo.Object,
                _orderLogger.Object,
                userService,
                _authService
            );
        }

        [Fact]
        public async Task AddAsync_Should_CreateOrder_WithProducts()
        {
            // Arrange
            var userGuid = Guid.NewGuid();
            var fixedProductGuid = Guid.Parse("a886caee-f30d-4c5d-8245-86ec297ba9b0");

            var product = new Product(
                id: fixedProductGuid.ToString(),
                name: "Produto Teste",
                description: "Descrição",
                price: 15,
                category: "Categoria",
                status: "enabled",
                storeId: Guid.NewGuid().ToString()
            );

            _productRepo
                .Setup(p => p.GetByIdAsync(
                    It.Is<ProductId>(pid => pid.AsGuid() == fixedProductGuid)
                ))
                .ReturnsAsync(product);

            _orderRepo
                .Setup(r => r.AddAsync(It.IsAny<Order>()))
                .ReturnsAsync((Order o) => o);

            _orderProductRepo
                .Setup(r => r.GetByOrderIdAsync(It.IsAny<OrderId>()))
                .ReturnsAsync((OrderId oid) => new List<OrderProduct>
                {
                    new(oid, new ProductId(fixedProductGuid), amount: 2, price: 15.5)
                });

            var dto = new CreatingOrderDto
            {
                UserId = userGuid,
                Status = "pending",
                Products = new List<CreatingOrderProductDto>
                {
                    new() { ProductId = fixedProductGuid, Amount = 2, Price = 15.5 }
                }
            };
            var userAuth = new AuthenticatedUserDto { Email = "test@example.com" };

            // Act
            var result = await _service.AddAsync(dto, userAuth);

            // Assert
            result.Should().NotBeNull();
            result.UserId.Should().Be(userGuid);
            result.Status.Should().Be("pending");
            result.Products.Should().HaveCount(1);

            _orderRepo.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Once);
            _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_Should_ReturnOrder_WithProducts()
        {
            // Arrange
            var id = new OrderId(Guid.NewGuid());
            var productId = new ProductId(Guid.NewGuid());
            var order = new Order(new UserId(Guid.NewGuid()), "processing");
            // populate navigation Products for sysAdmin path
            var prod = new OrderProduct(order.Id, productId, amount: 1, price: 10.0);
            // Use reflection or domain method if available
            typeof(Order)
                .GetProperty("Products", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)!
                .GetValue(order, null)
                .GetType()
                .GetMethod("Add")
                ?.Invoke(
                    typeof(Order)
                        .GetProperty("Products", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)!
                        .GetValue(order, null),
                    new object[] { prod }
                );

            // Given our fakeUser is System Administrator, service calls FindByIdWithProductsAsync
            _orderRepo
                .Setup(r => r.FindByIdWithProductsAsync(id))
                .ReturnsAsync(order);

            // also fallback for FindById
            _orderRepo
                .Setup(r => r.FindById(id))
                .ReturnsAsync(order);

            _orderProductRepo
                .Setup(r => r.GetByOrderIdAsync(id))
                .ReturnsAsync(new List<OrderProduct>
                {
                    new(id, productId, amount: 1, price: 10.0)
                });

            var userAuth = new AuthenticatedUserDto { Email = "test@example.com" };

            // Act
            var result = await _service.GetByIdAsync(id, userAuth);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be("processing");
            result.Products.Should().ContainSingle();
        }

        [Fact]
        public async Task PatchAsync_Should_UpdateStatus_And_ReplaceProducts()
        {
            // Arrange
            var id = new OrderId(Guid.NewGuid());
            var productGuid = Guid.NewGuid();
            var order = new Order(new UserId(Guid.NewGuid()), "pending");

            _orderRepo
                .Setup(r => r.FindById(id))
                .ReturnsAsync(order);

            _orderProductRepo
                .Setup(r => r.GetByOrderIdAsync(id))
                .ReturnsAsync(new List<OrderProduct>
                {
                    new(id, new ProductId(productGuid), amount: 1, price: 9.99)
                });

            _productRepo
                .Setup(p => p.GetByIdAsync(
                    It.Is<ProductId>(pid => pid.AsGuid() == productGuid)
                ))
                .ReturnsAsync(new Product(
                    id: productGuid.ToString(),
                    name: "Produto Patch",
                    description: "Descrição",
                    price: 9,
                    category: "Categoria",
                    status: "enabled",
                    storeId: Guid.NewGuid().ToString()
                ));

            var dto = new PartialOrderUpdateDto
            {
                Status = "delivered",
                Products = new List<CreatingOrderProductDto>
                {
                    new() { ProductId = productGuid, Amount = 1, Price = 9.99 }
                }
            };
            var userAuth = new AuthenticatedUserDto { Email = "test@example.com" };

            // Act
            var result = await _service.PatchAsync(id, dto, userAuth);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be("delivered");
            result.Products.Should().HaveCount(1);
            _orderProductRepo.Verify(r => r.DeleteByOrderIdAsync(id), Times.Once);
            _orderProductRepo.Verify(r => r.AddAsync(It.IsAny<OrderProduct>()), Times.Once);
            _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_Should_RemoveOrder()
        {
            // Arrange
            var id = new OrderId(Guid.NewGuid());
            var order = new Order(new UserId(Guid.NewGuid()), "cancelled");

            _orderRepo
                .Setup(r => r.FindById(id))
                .ReturnsAsync(order);

            var userAuth = new AuthenticatedUserDto { Email = "test@example.com" };

            // Act
            var result = await _service.DeleteAsync(id, userAuth);

            // Assert
            result.Should().BeTrue();
            _orderRepo.Verify(r => r.DeleteAsync(order), Times.Once);
            _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }
    }
}
