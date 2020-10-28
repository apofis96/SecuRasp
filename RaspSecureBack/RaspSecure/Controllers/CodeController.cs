using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaspSecure.Models;
using RaspSecure.Models.Auth;
using RaspSecure.Models.DTO;
using RaspSecure.Services;

namespace RaspSecure.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CodeController : ControllerBase
    {
        private readonly CodeService _codeService;

        public CodeController(CodeService codeService)
        {
            _codeService = codeService;
        }

        [HttpGet("{codeId}")]
        public async Task<ActionResult<IEnumerable<SecurityCode>>> GetAsync(int codeId)
        {
            try
            {
                return Ok(await _codeService.GetCodeAsync(codeId));
            }
            catch (ArgumentException e)
            {
                return NotFound(e);
            }
        }
        [Authorize(Roles = nameof(RolesEnum.Admin) + "," + nameof(RolesEnum.Editor))]
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<SecurityCode>>> GetAllAsync()
        {
            return Ok(await _codeService.GetAllAsync());
        }

        [HttpGet("checkcodesecurity/{code}")]
        public async Task<IActionResult> GetAsync(string code)
        {
            if (await _codeService.CheckCodeAsync(code))
                return Ok();
            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<SecurityCode>> PostCodeAsync([FromBody] SecurityCodeCreateDTO newCode)
        {
            try
            {
                return Ok(await _codeService.AddCodeAsync(newCode));
            }
            catch (ArgumentException e)
            {
                return BadRequest(e);
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
