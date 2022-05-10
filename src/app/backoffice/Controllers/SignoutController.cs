using Conventions.BackOffice.Models;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace Conventions.BackOffice.Controllers
{
    public class SignoutController : Controller
    {
        private readonly IOptions<SignoutControllerOptions> _options;

        public SignoutController(IOptions<SignoutControllerOptions> options)
        {
            _options = options;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            using var client = new HttpClient();
            var discoDocument = await client.GetDiscoveryDocumentAsync(_options.Value.Authority);
            var queryParams = new Dictionary<string, string>();
            var idToken = await HttpContext.GetTokenAsync("id_token");
            if (!string.IsNullOrEmpty(idToken))
            {
                queryParams.Add("id_token_hint", idToken);
            }

            if (!string.IsNullOrEmpty(_options.Value.RedirectUrl))
            {
                queryParams.Add("post_logout_redirect_uri", _options.Value.RedirectUrl);
            }

            var endpoint = QueryHelpers.AddQueryString(discoDocument.EndSessionEndpoint, queryParams);

            await HttpContext.SignOutAsync();
            return Redirect(endpoint);
        }
    }
}
