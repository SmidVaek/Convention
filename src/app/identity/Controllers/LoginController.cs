using Conventions.Identity.Models;
using Conventions.Models;
using Conventions.Services;
using IdentityServer4;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Conventions.Identity.Controllers
{
    [Route("login")]
    [AutoValidateAntiforgeryToken]
    public class LoginController : Controller
    {
        private readonly IUserService _userService;
        private readonly PasswordHasher<User> _passwordHasher = new();

        public LoginController(
            IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Index([FromQuery] string returnUrl)
        {
            // Check that returnUrl is set, else return 400
            if (!Request.Query.ContainsKey("returnUrl"))
            {
                throw new ArgumentNullException(nameof(returnUrl));
            }

            var model = new LoginViewModel()
            {
                ReturnUrl = returnUrl
            };

            // Make sure local authorize redirects are transformed to absolute, to handle agreement flows where it is called from the account server for instance
            if (model.ReturnUrl.StartsWith("/"))
            {
                model.ReturnUrl = HttpContext.GetIdentityServerOrigin() + model.ReturnUrl;
            }
            
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> OnPostAsync(LoginViewModel model)
        {
            // Make sure local authorize redirects are transformed to absolute, to handle agreement flows where it is called from the account server for instance
            if (model.ReturnUrl.StartsWith("/"))
            {
                model.ReturnUrl = HttpContext.GetIdentityServerOrigin() + model.ReturnUrl;
            }

            ArgumentNullException.ThrowIfNull(model.ReturnUrl);

            if (!ModelState.IsValid)
            {
                return View("index", model);
            }

            var user = await _userService.GetByEmailAsync(model.Username);
            if (user is null)
            {
                ModelState.AddModelError(nameof(model.Password), "Incorrect password or email, please try again.");
                return View("index", model);
            }

            if (string.IsNullOrEmpty(user.Password))
            {
                ModelState.AddModelError(nameof(model.Password), "Incorrect password or email, please try again.");
                return View("index", model);
            }

            var passwordValid = _passwordHasher.VerifyHashedPassword(user, user.Password, model.Password);
            if (passwordValid == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(nameof(model.Password), $"Incorrect password or email, please try again.");
                return View("index", model);
            }
            var claims = new List<Claim>();
            var identityUser = new IdentityServerUser(user.Id)
            {
                AdditionalClaims = claims,
                AuthenticationMethods = new[] { "pwd" }
            };

            var authenticationProperties = default(AuthenticationProperties);
            if (model.RememberMe)
            {
                authenticationProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(15)
                };
            }

            await HttpContext.SignInAsync(identityUser, authenticationProperties);

            return Redirect(model.ReturnUrl);
        }
    }
}
