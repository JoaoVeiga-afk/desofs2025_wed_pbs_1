using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using ShopTex.Domain.Shared;
using ShopTex.Domain.Products;
using ShopTex.Domain.Users;
using ShopTex.Services;

namespace ShopTex.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _service;
        private readonly AuthenticationService _authenticationService;

        public ProductController(ProductService service, AuthenticationService authenticationService)
        {
            _service = service;
            _authenticationService = authenticationService;
        }

        // GET: api/product/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<ProductDto>> GetProduct(Guid id)
        {
            var currentUserEmail =
                User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst("email")?.Value;

            if (string.IsNullOrWhiteSpace(currentUserEmail))
                return Unauthorized("User e-mail not found in token.");

            var userAuth = new AuthenticatedUserDto { Email = currentUserEmail };
            
            try
            {
                var product = await _service.GetProductByIdAsync(new ProductId(id), userAuth);

                if (product == null)
                {
                    return NotFound();
                }
                return Ok(product);
                
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
        
        // GET: api/product/
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<ProductDto>>> GetAllProducts()
        {
            var currentUserEmail =
                User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst("email")?.Value;

            if (string.IsNullOrWhiteSpace(currentUserEmail))
                return Unauthorized("User e-mail not found in token.");

            var userAuth = new AuthenticatedUserDto { Email = currentUserEmail };

            try
            {
                var products = await _service.GetAllProductsAsync(userAuth);

                if (products == null || products.Count == 0)
                    return NotFound(new { Message = "No products available." });

                return Ok(products);
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

        // POST: api/product/create
        [HttpPost]
        [Route("create")]
        [Authorize]
        public async Task<ActionResult<ProductDto>> CreateProduct(ProductDto dto)
        {
            try
            {
                var currentUserEmail =
                    User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst("email")?.Value;
                if (string.IsNullOrWhiteSpace(currentUserEmail))
                {
                    return Unauthorized("User email not found in token");
                }

                var authorized_sysadmin = await _authenticationService.hasPermission(currentUserEmail, new List<UserRole> { UserRole.SystemRole });
                var authorized_storeadmin = await _authenticationService.managesStore(currentUserEmail, dto.StoreId);
                var authorized_storecolab = await _authenticationService.worksOnStore(currentUserEmail, dto.StoreId);
                if (!(authorized_sysadmin || authorized_storeadmin || authorized_storecolab))
                {
                    return Unauthorized("You don't have permission to create a product");
                }

                var product = await _service.AddAsync(dto);

                return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("{id}/upload-image")]
        [Authorize]
        public async Task<IActionResult> UploadImage(string id, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var product = await _service.GetByIdAsync(new ProductId(id));
            if (product == null)
                return NotFound("Product not found.");

            // Validate user permissions here
            var currentUserEmail = User.Claims.FirstOrDefault(c => c.Type == "email")?.Value;

            if (string.IsNullOrWhiteSpace(currentUserEmail))
            {
                return Unauthorized("User email not found in token");
            }

            var authorized_sysadmin = await _authenticationService.hasPermission(currentUserEmail, new List<UserRole> { UserRole.SystemRole });
            var authorized_storeadmin = await _authenticationService.managesStore(currentUserEmail, id.ToString());
            var authorized_storecolab = await _authenticationService.worksOnStore(currentUserEmail, id.ToString());
            if (!(authorized_sysadmin || authorized_storeadmin || authorized_storecolab))
            {
                return Unauthorized("You don't have permission to create a product");
            }

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            var imageBytes = ms.ToArray();

            var success = await _service.UploadImage(id, imageBytes);

            if (!success)
                return StatusCode(500, "Failed to encrypt and save image.");

            return Ok("Image uploaded successfully.");
        }
    }
}