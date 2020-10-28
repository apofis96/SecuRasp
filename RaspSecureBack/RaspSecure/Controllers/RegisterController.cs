using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaspSecure.Models.Auth;
using RaspSecure.Models.DTO;
using RaspSecure.Services;
using System.Threading.Tasks;

namespace RaspSecure.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly AuthService _authService;

        public RegisterController(UserService userService, AuthService authService)
        {
            _userService = userService;
            _authService = authService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UserRegisterDTO userDTO)
        {
            var user = await _userService.CreateUser(userDTO);
            var token = await _authService.GenerateAccessToken(user.Id, user.UserName, user.Email, user.Role);

            var result = new AuthUserDTO
            {
                User = user,
                Token = token
            };

            return CreatedAtAction("GetById", "users", new { id = user.Id }, result);
        }

        [Authorize(Roles = nameof(RolesEnum.Admin))]
        [HttpPost("terminal")]
        public async Task<IActionResult> PostTerminal([FromBody] UserRegisterDTO userDTO)
        {
            userDTO.Role = RolesEnum.Terminal;
            return Ok(await _userService.CreateUser(userDTO, true));
        }

        [HttpGet]
        public async Task<IActionResult> GetAdminUser()
        {
            return Ok(await _userService.CheckAdminUser());
        }
    }
}
