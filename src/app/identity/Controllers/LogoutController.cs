using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Conventions.Identity.Controllers
{
    [Route("logout")]
    public class LogoutController : Controller
    {
        private readonly IIdentityServerInteractionService _identityserverInteractionService;

        public LogoutController(
            IIdentityServerInteractionService identityserverInteractionService)
        {
            _identityserverInteractionService = identityserverInteractionService;
        }

        [HttpGet]
        public async Task<IActionResult> OnGet([FromQuery] string logoutId)
        {
            var logoutContext = await _identityserverInteractionService.GetLogoutContextAsync(logoutId);

            if (User?.Identity?.IsAuthenticated ?? false)
            {
                await HttpContext.SignOutAsync();
                return Redirect(logoutContext.PostLogoutRedirectUri);
            }

            return View("SignedOut");
        }
    }
}
