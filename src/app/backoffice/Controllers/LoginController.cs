using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Conventions.BackOffice.Controllers
{
    public class LoginController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Challenge(new AuthenticationProperties() { RedirectUri = "/" }, "oidc");
        }
    }
}
