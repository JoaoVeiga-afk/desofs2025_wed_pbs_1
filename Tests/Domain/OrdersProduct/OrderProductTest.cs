using FluentAssertions;
using ShopTex.Domain.Orders;
using ShopTex.Domain.OrdersProduct;
using System;
using Xunit;

namespace ShopTex.Tests.Domain.OrdersProduct
{
    public class OrderProductTest
    {
        [Fact]
        public void Constructor_ShouldSetPropertiesCorrectly()
        {
            var orderId = new OrderId(Guid.NewGuid());
            var productId = 10;
            var amount = 3;
            var price = 49.99;

            var orderProduct = new OrderProduct(orderId, productId, amount, price);

            orderProduct.OrderId.Should().Be(orderId);
            orderProduct.ProductId.Should().Be(productId);
            orderProduct.Amount.Should().Be(amount);
            orderProduct.Price.Should().Be(price);
        }

        [Fact]
        public void Constructor_WithZeroAmount_ShouldAllowConstruction()
        {
            var orderId = new OrderId(Guid.NewGuid());

            var result = new OrderProduct(orderId, 1, 0, 9.99);

            result.Amount.Should().Be(0); 
        }

        [Fact]
        public void Constructor_WithNegativePrice_ShouldAllowConstruction()
        {
            var orderId = new OrderId(Guid.NewGuid());

            var result = new OrderProduct(orderId, 1, 1, -5.0);

            result.Price.Should().BeNegative(); 
        }
    }
}