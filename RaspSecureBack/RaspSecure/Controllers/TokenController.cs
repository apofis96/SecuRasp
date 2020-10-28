using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaspSecure.Extensions;
using RaspSecure.Models.DTO;
using RaspSecure.Services;
using System.Threading.Tasks;

namespace RaspSecure.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly AuthService _authService;

        public TokenController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<ActionResult<AccessTokenDTO>> Refresh([FromBody] RefreshTokenDTO tokenDTO)
        {
            return Ok(await _authService.RefreshToken(tokenDTO));
        }

        [HttpPost("revoke")]
        public async Task<IActionResult> RevokeRefreshToken([FromBody] RevokeRefreshTokenDTO tokenDTO)
        {
            var userId = this.GetUserIdFromToken();
            await _authService.RevokeRefreshToken(tokenDTO.RefreshToken, userId);
            return Ok();
        }
    }
}
