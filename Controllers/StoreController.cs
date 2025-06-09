using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using ShopTex.Domain.Shared;
using ShopTex.Domain.Stores;
using ShopTex.Domain.Users;
using ShopTex.Services;

namespace ShopTex.Controllers
{
    [Route("api/store")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        private readonly StoreService _service;
        private readonly AuthenticationService _authenticationService;

        public StoreController(StoreService service, AuthenticationService authenticationService)
        {
            _service = service;
            _authenticationService = authenticationService;
        }

        // GET: api/store/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StoreDto>> GetStore(Guid id)
        {
            var store = await _service.GetByIdAsync(new StoreId(id));

            if (store == null)
            {
                return NotFound();
            }

            return store;
        }

        // POST: api/store/create
        [HttpPost]
        [Route("create")]
        [Authorize(Roles = Configurations.SYS_ADMIN_ROLE_NAME)]
        public async Task<ActionResult<StoreDto>> CreateStore(CreatingStoreDto dto)
        {
            try
            {
                var currentUserEmail = User.Claims.FirstOrDefault(c => c.Type == "email")?.Value;

                if (string.IsNullOrWhiteSpace(currentUserEmail))
                {
                    return Unauthorized("User email not found in token");
                }

                var authorized = await _authenticationService.hasPermission(currentUserEmail, new List<UserRole> { UserRole.SystemRole });
                if (!authorized)
                {
                    return Unauthorized("You don't have permission to create a store");
                }

                var store = await _service.AddAsync(dto);

                return CreatedAtAction(nameof(GetStore), new { id = store.Id }, store);
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}