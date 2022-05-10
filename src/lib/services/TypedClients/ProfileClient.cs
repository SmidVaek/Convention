using Conventions.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Conventions.Services.TypedClients
{
    public class ProfileClient
    {
        private readonly HttpClient _httpClient;
        private readonly BlazorAccessTokenProvider _blazorAccessTokenProvider;
        private readonly string _endpointBase = "api/me";
        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };

        public ProfileClient(HttpClient httpClient, BlazorAccessTokenProvider blazorAccessTokenProvider)
        {
            _httpClient = httpClient;
            _blazorAccessTokenProvider = blazorAccessTokenProvider;
        }

        public async Task<User?> Get()
        {
            var endpoint = $"{_endpointBase}";
            var token = _blazorAccessTokenProvider.AccessToken;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return await _httpClient.GetFromJsonAsync<User?>(endpoint, _jsonSerializerOptions);
        }

        public async Task<User> CreateUser(User user)
        {
            var endpoint = $"{_endpointBase}";
            var result = await _httpClient.PostAsJsonAsync(endpoint, user);
            var content = await result.Content.ReadAsStringAsync();
            var createdUser = JsonSerializer.Deserialize<User>(content, _jsonSerializerOptions);
            return createdUser;
        }

    }
}
