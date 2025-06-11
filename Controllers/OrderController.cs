using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using NuGet.Protocol;
using ShopTex.Domain.Orders;
using ShopTex.Domain.Shared;
using ShopTex.Domain.Users;
using ShopTex.Services;

namespace ShopTex.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }
        
        // GET: api/order/4c656ea7-8e30-414d-8680-10229a796467
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<OrderDto>> GetOrder(Guid id)
        {
            var order = await _orderService.GetByIdAsync(new OrderId(id));
            
            if (order == null)
            {
                return NotFound();
            }

            return order;
        }
        
        // GET: api/order?limit=20&offset=0
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders(
            [FromQuery] int limit = 10,
            [FromQuery] int offset = 0)
        {
            if (limit <= 0 || offset < 0)
                return BadRequest("limit must be > 0 and offset >= 0");

            var dtos = await _orderService.GetAllAsync(offset, limit);
            return Ok(dtos);
        }
        
        // POST: api/order
        [HttpPost]
        [Authorize]        
        public async Task<ActionResult<OrderDto>> PostOrder([FromBody] CreatingOrderDto orderInfo)
        {
            var currentUserEmail =
                User.FindFirst(ClaimTypes.Email)?.Value ??
                User.FindFirst("email")?.Value;

            if (string.IsNullOrWhiteSpace(currentUserEmail))
                return Unauthorized("User e-mail not found in token.");

            var userAuth = new AuthenticatedUserDto { Email = currentUserEmail };

            try
            {
                var order = await _orderService.AddAsync(orderInfo, userAuth);
                return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        
        // PATCH: api/order/4c656ea7-8e30-414d-8680-10229a796467
        [HttpPatch("{id}")]
        [Authorize]
        public async Task<IActionResult> PatchOrder(Guid id, [FromBody] PartialOrderUpdateDto dto)
        {
            var currentUserEmail =
                User.FindFirst(ClaimTypes.Email)?.Value ??
                User.FindFirst("email")?.Value;

            if (string.IsNullOrWhiteSpace(currentUserEmail))
                return Unauthorized("User e-mail not found in token.");

            var userAuth = new AuthenticatedUserDto { Email = currentUserEmail };
            try
            {
                var updated = await _orderService.PatchAsync(new OrderId(id), dto, userAuth);
                if (updated == null)
                    return NotFound(new { Message = $"Order {id} not found." });

                return Ok(updated);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        
        // DELETE: api/order/4c656ea7-8e30-414d-8680-10229a796467
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            var currentUserEmail =
                User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst("email")?.Value;

            if (string.IsNullOrWhiteSpace(currentUserEmail))
                return Unauthorized("User e-mail not found in token.");

            var userAuth = new AuthenticatedUserDto { Email = currentUserEmail };

            try
            {
                var deleted = await _orderService.DeleteAsync(new OrderId(id), userAuth);
                if (!deleted)
                    return NotFound(new { message = $"Order {id} not found" });

                return Ok(new
                {
                    success = true,
                    message = $"Order {id} deleted successfully"
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}