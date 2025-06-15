using System.Security.Claims;
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
                var currentUserEmail =
                    User.FindFirst(ClaimTypes.Email)?.Value ??
                    User.FindFirst("email")?.Value;
                
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

        // POST: api/store/colab/add
        [HttpPost]
        [Route("colab/add")]
        [Authorize]
        public async Task<ActionResult> AddColaborator(AddCollabDto dto)
        {
            try
            {
                // Check if current user is System Administrator or Store Administrator of the given store
                var currentUserEmail =
                    User.FindFirst(ClaimTypes.Email)?.Value ??
                    User.FindFirst("email")?.Value;
                
                if (string.IsNullOrWhiteSpace(currentUserEmail))
                {
                    return Unauthorized("User email not found in token");
                }

                var authorized_sysadmin = await _authenticationService.hasPermission(currentUserEmail, new List<UserRole> { UserRole.SystemRole });
                var authorized_storeadmin = await _authenticationService.managesStore(currentUserEmail, dto.StoreId);
                if (!authorized_sysadmin && !authorized_storeadmin)
                {
                    return Unauthorized("You don't have permission to create a store");
                }

                var userIsColaborator = await _authenticationService.hasPermission(dto.UserEmail, new List<UserRole> { UserRole.StoreColabRole });
                if (!userIsColaborator)
                {
                    return BadRequest("Provided user is not a store colaborator");
                }

                // Add collaborator
                var (success, message) = await _service.AddStoreColaborator(dto);
                if (!success)
                {
                    return BadRequest(message);
                }

                return Ok(message);
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        
        // POST: api/store/client/add
        [HttpPost]
        [Route("client/add")]
        [Authorize]
        public async Task<ActionResult> AddClient(AddClientDto dto)
        {
            try
            {
                var currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst("email")?.Value;
                var role = User.FindFirst(ClaimTypes.Role)?.Value ?? User.FindFirst("role")?.Value;
                var userAuth = new AuthenticatedUserDto { Email = currentUserEmail };

                if (string.IsNullOrWhiteSpace(currentUserEmail))
                {
                    return Unauthorized("User email not found in token");
                }

                if (role == "Client")
                {
                    var addClientResult = await _service.AddStoreClient(dto, userAuth);

                    if (!addClientResult.Success)
                    {
                        return BadRequest(new { Message = addClientResult.Message });
                    }

                    return Ok(new { Message = "Client added to store successfully" });
                }
                
                var authorized_sysadmin = await _authenticationService.hasPermission(currentUserEmail, new List<UserRole> { UserRole.SystemRole });
                var authorized_storeadmin = await _authenticationService.managesStore(currentUserEmail, dto.StoreId.ToString());
                var userIsColaborator = await _authenticationService.hasPermission(currentUserEmail, new List<UserRole> { UserRole.StoreColabRole });

                if (!authorized_sysadmin && !authorized_storeadmin && !userIsColaborator)
                {
                    return Unauthorized("You don't have permission to add a user to store.");
                }

                if (!dto.UserId.HasValue)
                {
                    return BadRequest("UserId is required.");
                }

                var addClientResultForAdmin = await _service.AddStoreClient(dto, userAuth);

                if (!addClientResultForAdmin.Success)
                {
                    return BadRequest(new { Message = addClientResultForAdmin.Message });
                }

                return Ok(new { Message = "Client added to store successfully" });
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }


    }
}