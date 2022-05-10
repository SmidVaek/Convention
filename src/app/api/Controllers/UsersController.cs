using Conventions.Models;
using Conventions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace conventions.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "s2s")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet()]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user is null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpGet("role/{role}")]
        public async Task<IActionResult> GetUserByRole(string role)
        {
            var users = await _userService.GetByRoleAsync(role);
            return Ok(users);
        }


        [HttpPut]
        public async Task<IActionResult> EditUser([FromBody] User user)
        {
            await _userService.UpdateAsync(user);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUser(string id)
        {
            await _userService.DeleteAsync(id);
            return Ok();
        }
    }
}
