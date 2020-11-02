using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaspSecure.Extensions;
using RaspSecure.Models.Auth;
using RaspSecure.Models.DTO;
using RaspSecure.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RaspSecure.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<UsersListD>> Get(string sort = "asc", int page = 0, int size = 10, bool unactive = false)
        {
            return Ok(await _userService.GetUsers(sort, page, size, unactive));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<UserDTO>> GetById(int id)
        {
            return Ok(await _userService.GetUserById(id));
        }

        [HttpGet("fromToken")]
        public async Task<ActionResult<UserDTO>> GetUserFromToken()
        {
            return Ok(await _userService.GetUserById(this.GetUserIdFromToken()));
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] UserDTO user)
        {
            await _userService.UpdateUser(user, this.GetUserIdFromToken());
            return NoContent();
        }

        [Authorize(Roles = nameof(RolesEnum.Admin))]
        [HttpPut("adminEdit")]
        public async Task<IActionResult> PutEdmin([FromBody] UserDTO user)
        {
            await _userService.AdminUserEdit(user, this.GetUserIdFromToken());
            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _userService.DeleteUser(id);
            return NoContent();
        }
    }
}
