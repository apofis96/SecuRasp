using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaspSecure.Extensions;
using RaspSecure.Models.DTO;
using RaspSecure.Services;
using System.Threading.Tasks;

namespace RaspSecure.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthUserDTO>> Login(UserLoginDTO userDTO)
        {
            return Ok(await _authService.Authorize(userDTO));
        }

        [HttpPost("reset/{email:regex(^[[a-zA-Z0-9.!#$%&’*+/=?^_`{{|}}~-]]+@[[a-zA-Z0-9-]]+(?:\\.[[a-zA-Z0-9-]]+)*$)}")]
        public async Task<IActionResult> Reset(string email, string host)
        {
            await _authService.Reset(email, host);
            return Ok();
        }

        [HttpPut("update/{password:regex(^(?=.*[[A-Za-z]])(?=.*\\d)[[A-Za-z\\d]]{{8,}}$)}")] //8 char one AZ one 09
        public async Task<IActionResult> Update(string password, string token)
        {
            await _authService.Update(password, token);
            return Ok();
        }

        [Authorize]
        [HttpPut("edit/{password:regex(^(?=.*[[A-Za-z]])(?=.*\\d)[[A-Za-z\\d]]{{8,}}$)}")] //8 char one AZ one 09
        public async Task<IActionResult> Edit(string password)
        {
            var userId = this.GetUserIdFromToken();
            await _authService.Update(password, userId);
            return Ok();
        }
    }
}
