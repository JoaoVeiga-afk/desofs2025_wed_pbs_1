using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using ShopTex.Domain.Orders;
using ShopTex.Domain.OrdersProduct;
using ShopTex.Domain.Products;
using ShopTex.Domain.Shared;
using ShopTex.Domain.Stores;
using ShopTex.Domain.Users;

namespace ShopTex.Services;

public class OrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOrderRepository _repo;
    private readonly IOrderProductRepository _orderProductRepo;
    private readonly IProductRepository _produtRepo;
    private readonly ILogger<OrderService> _logger;
    private readonly UserService _userService;
    private readonly AuthenticationService _authenticationService;

    public OrderService(
        IUnitOfWork unitOfWork,
        IOrderRepository repo,
        IOrderProductRepository orderProductRepo,
        IProductRepository produtRepo,
        ILogger<OrderService> logger,
        UserService userService,
        AuthenticationService authenticationService)
    {
        _unitOfWork = unitOfWork;
        _repo = repo;
        _orderProductRepo = orderProductRepo;
        _produtRepo = produtRepo;
        _logger = logger;
        _userService = userService;
        _authenticationService = authenticationService;
    }

    public async Task<OrderDto?> GetByIdAsync(OrderId id, AuthenticatedUserDto userAuth)
    {
        _logger.LogInformation("Fetching order with ID {OrderId}", id);
        await ValidateUserAccessAsync(userAuth);

        var user = await _userService.GetUserByEmailAsync(userAuth.Email);

        var isSysAdmin   = await _authenticationService.hasPermission(userAuth.Email,   new List<UserRole>{ UserRole.SystemRole });
        var isStoreAdmin = await _authenticationService.managesStore(userAuth.Email,   user.Store!.AsGuid().ToString());
        var isStoreColab = await _authenticationService.worksOnStore(userAuth.Email,    user.Store!.AsGuid().ToString());

        Order? order;
        if (isSysAdmin)
        {
            order = await _repo.FindByIdWithProductsAsync(id);
        }
        else if (isStoreAdmin || isStoreColab)
        {
            var storeVo = new StoreId(user.Store!.AsGuid().ToString());
            order = await _repo.FindByIdByStoreAsync(storeVo, id);
        }
        else
        {
            var userVo = new UserId(user.Id.AsGuid());
            order = await _repo.FindByIdByUserAsync(userVo, id);
        }

        if (order == null)
        {
            _logger.LogWarning("Order with ID {OrderId} not found or access denied", id);
            return null;
        }

        // Busca sempre os produtos associados ao pedido
        var products = await _orderProductRepo.GetByOrderIdAsync(id);

        return new OrderDto
        {
            Id        = order.Id.AsGuid(),
            UserId    = order.UserId.AsGuid(),
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


    public async Task<List<OrderDto>> GetAllAsync(int offset, int limit, AuthenticatedUserDto userAuth)
    {
        _logger.LogInformation("Fetching orders offset {Offset} limit {Limit}", offset, limit);
        await ValidateUserAccessAsync(userAuth);

        var user = await _userService.GetUserByEmailAsync(userAuth.Email)
                   ?? throw new BusinessRuleValidationException("User not found");

        var isSysAdmin    = await _authenticationService.hasPermission(userAuth.Email, new List<UserRole>{ UserRole.SystemRole });
        var isStoreAdmin  = await _authenticationService.managesStore(userAuth.Email,   user.Store!.AsGuid().ToString());
        var isStoreColab  = await _authenticationService.worksOnStore(userAuth.Email,    user.Store!.AsGuid().ToString());

        List<Order> orders;
        if (isSysAdmin)
        {
            orders = await _repo.GetPagedAsync(offset, limit);
        }
        else if (isStoreAdmin || isStoreColab)
        {
            var storeGuid = user.Store!.AsGuid();
            orders = await _repo.GetPagedByStoreAsync(storeGuid, offset, limit);
        }
        else
        {
            var userGuid = user.Id.AsGuid();
            orders = await _repo.GetPagedByUserAsync(userGuid, offset, limit);
        }

        var results = new List<OrderDto>();
        foreach (var order in orders)
        {
            var products = await _orderProductRepo.GetByOrderIdAsync(order.Id);
            results.Add(new OrderDto {
                Id        = order.Id.AsGuid(),
                UserId    = order.UserId.AsGuid(),
                Status    = order.Status.ToString(),
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
                Products  = products.Select(p => new OrderProductDto {
                    ProductId = p.ProductId,
                    Amount    = p.Amount,
                    Price     = p.Price
                }).ToList()
            });
        }

        return results;
    }

    public async Task<OrderDto> AddAsync(CreatingOrderDto dto, AuthenticatedUserDto userAuth)
    {
        await ValidateUserAccessAsync(userAuth);
        var userId = await ResolveAndValidateUserIdAsync(dto.UserId, userAuth);

        foreach (var p in dto.Products)
        {
            if (!p.ProductId.HasValue)
                throw new BusinessRuleValidationException("ProductId is required.");

            var productId = new ProductId(p.ProductId.Value);
            var existingProduct = await _produtRepo.GetByIdAsync(productId);
            if (existingProduct == null)
                throw new BusinessRuleValidationException($"Product {p.ProductId} does not exist.");
        }

        var grouped = dto.Products
            .GroupBy(p => p.ProductId!.Value)
            .Select(g => new
            {
                ProductId = g.Key,
                Amount    = g.Sum(x => x.Amount),
                Price     = g.Average(x => x.Price)
            })
            .ToList();

        var order = new Order(userId, dto.Status);
        await _repo.AddAsync(order);

        foreach (var item in grouped)
        {
            var productId = new ProductId(item.ProductId);
            var op = new OrderProduct(order.Id, productId, item.Amount, item.Price);
            await _orderProductRepo.AddAsync(op);
        }
        await _unitOfWork.CommitAsync();

        var persisted = await _orderProductRepo.GetByOrderIdAsync(order.Id);

        return new OrderDto
        {
            Id        = order.Id.AsGuid(),
            UserId    = order.UserId.AsGuid(),
            Status    = order.Status.ToString(),
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            Products  = persisted.Select(op => new OrderProductDto
            {
                ProductId = op.ProductId,
                Amount    = op.Amount,
                Price     = op.Price
            }).ToList()
        };
    }

    public async Task<OrderDto> PatchAsync(OrderId id, PartialOrderUpdateDto dto, AuthenticatedUserDto userAuth)
    {
        _logger.LogInformation("Patching order {OrderId}", id);
        await ValidateUserAccessAsync(userAuth);

        var order = await _repo.FindById(id);
        if (order == null)
            throw new BusinessRuleValidationException($"Order {id} not found");

        if (!string.IsNullOrWhiteSpace(dto.Status))
            order.SetStatus(dto.Status);

        if (dto.Products != null)
        {
            await _orderProductRepo.DeleteByOrderIdAsync(id);

            var grouped = dto.Products
                .GroupBy(p => p.ProductId!.Value)
                .Select(g => new
                {
                    ProductId = g.Key,
                    Amount    = g.Sum(x => x.Amount),
                    Price     = g.Average(x => x.Price)
                })
                .ToList();

            foreach (var item in grouped)
            {
                var productId = new ProductId(item.ProductId);
                var existingProduct = await _produtRepo.GetByIdAsync(productId);
                if (existingProduct == null)
                    throw new BusinessRuleValidationException($"Product {item.ProductId} does not exist.");

                var op = new OrderProduct(id, productId, item.Amount, item.Price);
                await _orderProductRepo.AddAsync(op);
            }
        }


        await _unitOfWork.CommitAsync();
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

    public async Task<bool> DeleteAsync(OrderId id, AuthenticatedUserDto userAuth)
    {
        _logger.LogInformation("Deleting order {OrderId}", id);

        var order = await _repo.FindById(id);
        if (order == null)
        {
            _logger.LogWarning("Order {OrderId} not found", id);
            return false;
        }

        
        var user = await _userService.GetUserByEmailAsync(userAuth.Email)
                   ?? throw new BusinessRuleValidationException("Authenticated user not found.");

        if (order.UserId.AsGuid() == user.Id.AsGuid())
        {
            await _repo.DeleteAsync(order);
            await _unitOfWork.CommitAsync();
            _logger.LogInformation("Order {OrderId} deleted by owner {UserId}", id, user.Id);
            return true;
        }

        var isSysAdmin   = await _authenticationService.hasPermission(
                               userAuth.Email,
                               new List<UserRole>{ UserRole.SystemRole }
                           );
        var storeId      = user.Store!.AsGuid().ToString();
        var managesStore = await _authenticationService.managesStore(
                               userAuth.Email,
                               storeId
                           );
        var worksOnStore = await _authenticationService.worksOnStore(
                               userAuth.Email,
                               storeId
                           );

        if (!isSysAdmin && !managesStore && !worksOnStore)
        {
            _logger.LogWarning("User {Email} is not allowed to delete order {OrderId}", userAuth.Email, id);
            throw new UnauthorizedAccessException("You don't have permission to delete this order.");
        }

        await _repo.DeleteAsync(order);
        await _unitOfWork.CommitAsync();
        _logger.LogInformation("Order {OrderId} deleted by user {Email}", id, userAuth.Email);
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
        Console.WriteLine($"User Email: {userAuth.Email}");
        Console.WriteLine($"hasPermission: {hasPermission}");

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

    private async Task<UserId> ResolveAndValidateUserIdAsync(Guid? dtoUserId, AuthenticatedUserDto userCtx)
    {
        if (!dtoUserId.HasValue || dtoUserId == Guid.Empty)
        {
            var user = await _userService.GetUserByEmailAsync(userCtx.Email);
            if (user == null)
            {
                _logger.LogWarning("User with e-mail {Email} not found", userCtx.Email);
                throw new BusinessRuleValidationException("User not found.");
            }
            dtoUserId = user.Id.AsGuid();
        }

        if (dtoUserId == Guid.Empty)
            throw new BusinessRuleValidationException("UserId is required.");

        var candidate = new UserId(dtoUserId.Value);
        var existing  = await _userService.GetByIdAsync(candidate);
        if (existing == null)
        {
            _logger.LogWarning("User with ID {UserId} does not exist", candidate);
            throw new BusinessRuleValidationException("User not found.");
        }
        return candidate;
    }
}
