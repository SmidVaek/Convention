using Conventions.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Conventions.Services.TypedClients
{
    public class UserClient
    {
        private readonly HttpClient _httpClient;
        private readonly BackOfficeServerTokenClient _backOfficeServerTokenClient;

        public UserClient(HttpClient httpClient,
                          BackOfficeServerTokenClient backOfficeServerTokenClient)
        {
            _httpClient = httpClient;
            _backOfficeServerTokenClient = backOfficeServerTokenClient;
        }

        public async Task<IEnumerable<User>?> GetUsersAsync()
        {
            var endpoint = "api/users";
            var token = await _backOfficeServerTokenClient.GetTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return await _httpClient.GetFromJsonAsync<User[]>(endpoint);
        }

        public async Task<User?> GetUserAsync(string id)
        {
            var endpoint = $"api/users/{id}";
            var token = await _backOfficeServerTokenClient.GetTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return await _httpClient.GetFromJsonAsync<User?>(endpoint);
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(string role)
        {
            var endpoint = $"api/users/role/{role}";
            var token = await _backOfficeServerTokenClient.GetTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return await _httpClient.GetFromJsonAsync<User[]>(endpoint);
        }
    }
}
