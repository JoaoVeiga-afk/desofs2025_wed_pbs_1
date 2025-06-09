using Microsoft.Extensions.Logging;
using ShopTex.Domain.Orders;
using ShopTex.Domain.OrdersProduct;
using ShopTex.Domain.Shared;
using ShopTex.Domain.Users;

namespace ShopTex.Services;

public class OrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOrderRepository _repo;
    private readonly IOrderProductRepository _orderProductRepo;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        IUnitOfWork unitOfWork,
        IOrderRepository repo,
        IOrderProductRepository orderProductRepo,
        ILogger<OrderService> logger)
    {
        _unitOfWork = unitOfWork;
        _repo = repo;
        _orderProductRepo = orderProductRepo;
        _logger = logger;
    }

    public async Task<OrderDto?> GetByIdAsync(OrderId id)
    {
        _logger.LogInformation("Fetching order with ID {OrderId}", id);

        var order = await _repo.FindById(id);
        if (order == null)
        {
            _logger.LogWarning("Order with ID {OrderId} not found", id);
            return null;
        }

        var products = await _orderProductRepo.GetByOrderIdAsync(id);

        return new OrderDto
        {
            Id = order.Id.AsGuid(),
            UserId = order.UserId.AsGuid(),
            Status = order.Status.ToString(),
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            Products = products.Select(p => new OrderProductDto
            {
                ProductId = p.ProductId,
                Amount = p.Amount,
                Price = p.Price
            }).ToList()
        };
    }

    public async Task<List<OrderDto>> GetAllAsync(int offset, int limit)
    {
        _logger.LogInformation("Fetching orders offset {Offset} limit {Limit}", offset, limit);

        var orders = await _repo.GetPagedAsync(offset, limit);

        var results = new List<OrderDto>();
        foreach (var order in orders)
        {
            var products = await _orderProductRepo.GetByOrderIdAsync(order.Id);

            results.Add(new OrderDto
            {
                Id        = order.Id.AsGuid(),
                UserId = order.UserId.AsGuid(),
                Status    = order.Status.ToString(),
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
                Products  = products.Select(p => new OrderProductDto
                {
                    ProductId = p.ProductId,
                    Amount    = p.Amount,
                    Price     = p.Price
                }).ToList()
            });
        }

        return results;
    }

    public async Task<OrderDto> AddAsync(CreatingOrderDto dto)
    {
        _logger.LogInformation("Creating order for user {UserId}", dto.UserId);

        var userId = new UserId(dto.UserId);
        var order = new Order(userId, dto.Status);
        foreach (var productDto in dto.Products)
        {
            order.AddProduct(productDto.ProductId, productDto.Amount, productDto.Price);
        }

        try
        {
            await _repo.AddAsync(order);
            await _unitOfWork.CommitAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create order");
            throw new BusinessRuleValidationException("Failed to create order");
        }

        _logger.LogInformation("Order {OrderId} created successfully", order.Id);

        return new OrderDto
        {
            Id = order.Id.AsGuid(),
            UserId = order.UserId.AsGuid(),
            Status = order.Status.ToString(),
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            Products = order.Products.Select(p => new OrderProductDto
            {
                ProductId = p.ProductId,
                Amount = p.Amount,
                Price = p.Price
            }).ToList()
        };
    }
    
    public async Task<OrderDto> PatchAsync(OrderId id, PartialOrderUpdateDto dto)
    {
        _logger.LogInformation("Patching order {OrderId}", id);

        var order = await _repo.FindById(id);
        if (order == null)
            throw new BusinessRuleValidationException($"Order {id} not found");

        if (!string.IsNullOrWhiteSpace(dto.Status))
            order.SetStatus(dto.Status);

        if (dto.Products != null)
        {
            await _orderProductRepo.DeleteByOrderIdAsync(id);

            foreach (var p in dto.Products)
            {
                var op = new OrderProduct(id, p.ProductId, p.Amount, p.Price);
                await _orderProductRepo.AddAsync(op);
            }
        }

        await _unitOfWork.CommitAsync();

        var products = await _orderProductRepo.GetByOrderIdAsync(id);

        return new OrderDto
        {
            Id        = order.Id.AsGuid(),
            UserId = order.UserId.AsGuid(),
            Status    = order.Status.ToString(),
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            Products  = products
                .Select(p => new OrderProductDto {
                    ProductId = p.ProductId,
                    Amount    = p.Amount,
                    Price     = p.Price
                })
                .ToList()
        };
    }


    
    public async Task<bool> DeleteAsync(OrderId id)
    {
        _logger.LogInformation("Deleting order {OrderId}", id);

        var order = await _repo.FindById(id);
        if (order == null)
        {
            _logger.LogWarning("Order {OrderId} not found", id);
            return false;
        }

        await _repo.DeleteAsync(order);

        await _unitOfWork.CommitAsync();

        _logger.LogInformation("Order {OrderId} deleted", id);
        return true;
    }

}
