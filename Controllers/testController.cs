using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using ShopTex.Domain.Shared;
using ShopTex.Domain.Users;
using ShopTex.Services;

//dotnet aspnet-codegenerator controller -name UserController -async -api -m User -dc DatabaseContext -outDir Controllers -f
namespace ShopTex.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class testController : ControllerBase
    {
        private readonly UserService _service;

        public testController(UserService service)
        {
            _service = service;
        }

        /*// GET: api/User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            return await _service.GetAllAsync();
        }*/

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(Guid id)
        {
            var user = await _service.GetByIdAsync(new UserId(id));

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpGet]
        [Route("me")]
        public async Task<ActionResult<dynamic>> GetMe()
        {
            return User.ToJson();
        }

        // POST: api/auth/signup
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Route("signup")]
        public async Task<ActionResult<UserDto>> PostUser(CreatingUserDto userInfo)
        {
            try
            {
                var user = await _service.AddAsync(userInfo);

                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        
        // api/auth/signin
        [HttpPost]
        [Route("signin")]
        public async Task<ActionResult<dynamic>> SignInUser(UserSignInDto dto)
        {
            var user = await _service.UserSignIn(dto);

            if (user == null) return BadRequest("Email or Password incorrect");

            return  user;
        }
        
    }
}
