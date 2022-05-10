using IdentityModel.Client;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Conventions.Services.TypedClients
{
    public class BackOfficeServerTokenClient
    {
        private readonly HttpClient _httpClient;
        private readonly IOptions<BackOfficeServerTokenClientOptions> _options;
        private readonly IMemoryCache _cache;

        public BackOfficeServerTokenClient(HttpClient httpClient, IOptions<BackOfficeServerTokenClientOptions> options, IMemoryCache cache)
        {
            _httpClient = httpClient;
            _options = options;
            _cache = cache;
        }

        public async Task<string> GetTokenAsync()
        {
            var cacheKey = $"{_options.Value.Authority}::{_options.Value.ClientId}";
            var token = await _cache.GetOrCreateAsync(cacheKey ,async factory =>
            {
                var discoDocument = await _httpClient.GetDiscoveryDocumentAsync(_options.Value.Authority);
                var tokenResponse = await _httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest()
                {
                    Address = discoDocument.TokenEndpoint,
                    ClientId = _options.Value.ClientId,
                    ClientSecret = _options.Value.ClientSecret,
                    Scope = "backoffice.s2s"
                });

                tokenResponse.HttpResponse.EnsureSuccessStatusCode();

                factory.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(tokenResponse.ExpiresIn);
                return tokenResponse.AccessToken;
            });

            return token;
        }
    }
}
