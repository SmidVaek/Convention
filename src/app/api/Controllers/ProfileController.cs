using Conventions.Models;
using Conventions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Conventions.API.Controllers
{
    [ApiController]
    [Authorize(Policy = "oidc")]
    [Route("api/me")]
    public class ProfileController : ControllerBase
    {
        private readonly IUserService _userService;

        public ProfileController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Get()
        {
            var idStr = User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var user = await _userService.GetByIdAsync(idStr);
            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost("")]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            var createdUser = await _userService.CreateAsync(user);
            return new CreatedResult($"id/{createdUser.Id}", createdUser);
        }
    }
}
