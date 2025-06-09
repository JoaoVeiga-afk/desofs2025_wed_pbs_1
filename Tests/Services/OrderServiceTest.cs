using FluentAssertions;
using Moq;
using ShopTex.Domain.Orders;
using ShopTex.Domain.OrdersProduct;
using ShopTex.Domain.Shared;
using ShopTex.Domain.Users;
using ShopTex.Services;
using Xunit;

namespace ShopTex.Tests.Services;

public class OrderServiceTest
{
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IOrderRepository> _orderRepository = new();
    private readonly Mock<IOrderProductRepository> _orderProductRepo = new();
    private readonly Mock<ILogger<OrderService>> _logger = new();

    private readonly OrderService _service;

    public OrderServiceTest()
    {
        _service = new OrderService(
            _unitOfWork.Object,
            _orderRepository.Object,
            _orderProductRepo.Object,
            _logger.Object);
    }

    [Fact]
    public async Task AddAsync_Should_CreateOrder_WithProducts()
    {
        var userGuid = Guid.NewGuid();
        var userId = new UserId(userGuid); 
        var dto = new CreatingOrderDto
        {
            UserId = userGuid,
            Status = "pending",
            Products = new List<CreatingOrderProductDto>
            {
                new() { ProductId = 1, Amount = 2, Price = 15.5 }
            }
        };

        var result = await _service.AddAsync(dto);

        result.Should().NotBeNull();
        result.UserId.Should().Be(userGuid); 
        result.Status.Should().Be("pending");
        result.Products.Should().HaveCount(1);

        _orderRepository.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Once);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }


    [Fact]
    public async Task GetByIdAsync_Should_ReturnOrder_WithProducts()
    {
        var id = new OrderId(Guid.NewGuid());
        var order = new Order(new UserId(Guid.NewGuid()), "processing");

        _orderRepository.Setup(r => r.FindById(id)).ReturnsAsync(order);
        _orderProductRepo.Setup(r => r.GetByOrderIdAsync(id)).ReturnsAsync(new List<OrderProduct>
        {
            new(id, 1, 1, 10.0)
        });

        var result = await _service.GetByIdAsync(id);

        result.Should().NotBeNull();
        result.Status.Should().Be("processing");
        result.Products.Should().ContainSingle();
    }

    [Fact]
    public async Task PatchAsync_Should_UpdateStatus_And_ReplaceProducts()
    {
        var id = new OrderId(Guid.NewGuid());
        var order = new Order(new UserId(Guid.NewGuid()), "pending");

        _orderRepository.Setup(r => r.FindById(id)).ReturnsAsync(order);
    
        _orderProductRepo.Setup(r => r.GetByOrderIdAsync(id)).ReturnsAsync(new List<OrderProduct>
        {
            new(id, 1, 1, 9.99)
        });

        var dto = new PartialOrderUpdateDto
        {
            Status = "delivered",
            Products = new List<CreatingOrderProductDto>
            {
                new() { ProductId = 1, Amount = 1, Price = 9.99 }
            }
        };

        var result = await _service.PatchAsync(id, dto);

        result.Status.Should().Be("delivered");

        _orderProductRepo.Verify(r => r.DeleteByOrderIdAsync(id), Times.Once);
        _orderProductRepo.Verify(r => r.AddAsync(It.IsAny<OrderProduct>()), Times.Once);
        _unitOfWork.Verify(r => r.CommitAsync(), Times.Once);
    }


    [Fact]
    public async Task DeleteAsync_Should_RemoveOrder()
    {
        var id = new OrderId(Guid.NewGuid());
        var order = new Order(new UserId(Guid.NewGuid()), "cancelled");

        _orderRepository.Setup(r => r.FindById(id)).ReturnsAsync(order);

        var result = await _service.DeleteAsync(id);

        result.Should().BeTrue();
        _orderRepository.Verify(r => r.DeleteAsync(order), Times.Once);
        _unitOfWork.Verify(r => r.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_Should_ReturnFalse_WhenNotFound()
    {
        var id = new OrderId(Guid.NewGuid());
        _orderRepository.Setup(r => r.FindById(id)).ReturnsAsync((Order?)null);

        var result = await _service.DeleteAsync(id);

        result.Should().BeFalse();
        _unitOfWork.Verify(r => r.CommitAsync(), Times.Never);
    }
}
