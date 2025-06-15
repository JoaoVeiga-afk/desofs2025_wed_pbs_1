using System;
using System.Linq;
using FluentAssertions;
using ShopTex.Domain.Orders;
using ShopTex.Domain.Products;
using ShopTex.Domain.Users;
using Xunit;

namespace ShopTex.Tests.Domain.Orders;

public class OrderTests
{
    [Fact]
    public void Constructor_Should_SetProperties()
    {
        var userId = new UserId(Guid.NewGuid());
        var status = "pending";

        var order = new Order(userId, status);

        order.UserId.Should().Be(userId);
        order.Status.ToString().Should().Be("pending");
        order.Products.Should().BeEmpty();
        order.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        order.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void AddProduct_Should_AddProductAndUpdateTimestamp()
    {
        var userId = new UserId(Guid.NewGuid());
        var order = new Order(userId, "pending");
        var productId = new ProductId(Guid.NewGuid());

        var before = order.UpdatedAt;
        order.AddProduct(productId: productId, amount: 2, price: 9.99);

        order.Products.Should().HaveCount(1);
        order.Products.First().ProductId.Should().Be(productId);
        order.Products.First().Amount.Should().Be(2);
        order.Products.First().Price.Should().Be(9.99);
        order.UpdatedAt.Should().BeAfter(before);
    }

    [Fact]
    public void SetStatus_Should_UpdateStatusAndTimestamp()
    {
        var userId = new UserId(Guid.NewGuid());
        var order = new Order(userId, "pending");

        var before = order.UpdatedAt;
        order.SetStatus("processing");

        order.Status.ToString().Should().Be("processing");
        order.UpdatedAt.Should().BeAfter(before);
    }
}