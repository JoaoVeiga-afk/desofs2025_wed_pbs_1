// Tests/Services/OrderServiceTest.cs
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
        private readonly Mock<IUnitOfWork>               _unitOfWork       = new();
        private readonly Mock<IOrderRepository>          _orderRepo        = new();
        private readonly Mock<IOrderProductRepository>   _orderProductRepo = new();
        private readonly Mock<IProductRepository>        _productRepo      = new();
        private readonly Mock<ILogger<OrderService>>     _orderLogger      = new();
        private readonly Mock<IUserRepository>           _userRepo         = new();
        private readonly Mock<IConfiguration>            _configuration    = new();
        private readonly Mock<ILogger<UserService>>      _userLogger       = new();
        private readonly Mock<ILogger<UserService>>      _authLogger       = new();

        private readonly AuthenticationService _authService;
        private readonly OrderService          _service;

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

            _productRepo
                .Setup(p => p.FindById(It.IsAny<string>()))
                .ReturnsAsync(new Product(Guid.NewGuid().ToString(), "Test", "Desc", 10, "Cat", "enabled", Guid.NewGuid().ToString()));

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
            var userGuid = Guid.NewGuid();
            var productGuid = Guid.NewGuid();
            var dto = new CreatingOrderDto
            {
                UserId   = userGuid,
                Status   = "pending",
                Products = new List<CreatingOrderProductDto> {
                    new() { ProductId = productGuid, Amount = 2, Price = 15.5 }
                }
            };
            var userAuth = new AuthenticatedUserDto { Email = "test@example.com" };

            var result = await _service.AddAsync(dto, userAuth);

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
            var id    = new OrderId(Guid.NewGuid());
            var productId = new ProductId(Guid.NewGuid());
            var order = new Order(new UserId(Guid.NewGuid()), "processing");

            _orderRepo.Setup(r => r.FindById(id)).ReturnsAsync(order);
            _orderProductRepo
                .Setup(r => r.GetByOrderIdAsync(id))
                .ReturnsAsync(new List<OrderProduct> { new(id, productId, 1, 10.0) });

            var result = await _service.GetByIdAsync(id);

            result.Should().NotBeNull();
            result.Status.Should().Be("processing");
            result.Products.Should().ContainSingle();
        }

        [Fact]
        public async Task PatchAsync_Should_UpdateStatus_And_ReplaceProducts()
        {
            var id    = new OrderId(Guid.NewGuid());
            var productId = Guid.NewGuid();
            var order = new Order(new UserId(Guid.NewGuid()), "pending");

            _orderRepo.Setup(r => r.FindById(id)).ReturnsAsync(order);
            _orderProductRepo
                .Setup(r => r.GetByOrderIdAsync(id))
                .ReturnsAsync(new List<OrderProduct> { new(id, new ProductId(productId), 1, 9.99) });

            var dto = new PartialOrderUpdateDto
            {
                Status   = "delivered",
                Products = new List<CreatingOrderProductDto> {
                    new() { ProductId = productId, Amount = 1, Price = 9.99 }
                }
            };
            var userAuth = new AuthenticatedUserDto { Email = "test@example.com" };

            var result = await _service.PatchAsync(id, dto, userAuth);

            result.Status.Should().Be("delivered");
            _orderProductRepo.Verify(r => r.DeleteByOrderIdAsync(id), Times.Once);
            _orderProductRepo.Verify(r => r.AddAsync(It.IsAny<OrderProduct>()), Times.Once);
            _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_Should_RemoveOrder()
        {
            var id    = new OrderId(Guid.NewGuid());
            var order = new Order(new UserId(Guid.NewGuid()), "cancelled");

            _orderRepo.Setup(r => r.FindById(id)).ReturnsAsync(order);
            var userAuth = new AuthenticatedUserDto { Email = "test@example.com" };

            var result = await _service.DeleteAsync(id, userAuth);

            result.Should().BeTrue();
            _orderRepo.Verify(r => r.DeleteAsync(order), Times.Once);
            _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }
    }
}
