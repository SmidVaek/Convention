using Conventions.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Conventions.Services.TypedClients
{
    public class VenueClient
    {
        private readonly HttpClient _httpClient;
        private readonly BackOfficeServerTokenClient _backOfficeServerTokenClient;
        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        public VenueClient(HttpClient httpClient,
                           BackOfficeServerTokenClient backOfficeServerTokenClient)
        {
            _httpClient = httpClient;
            _backOfficeServerTokenClient = backOfficeServerTokenClient;
        }

        public async Task<IEnumerable<Venue>> GetVenuesAsync()
        {
            var endpoint = "api/venues";
            var token = await _backOfficeServerTokenClient.GetTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(content))
            {
                return Array.Empty<Venue>();
            }

            var venues = JsonSerializer.Deserialize<List<Venue>>(content, _jsonSerializerOptions);
            return venues;
        }

        public async Task<Venue?> GetVenueAsync(string id)
        {
            var endpoint = $"api/venues/{id}";
            var token = await _backOfficeServerTokenClient.GetTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(content))
            {
                return null;
            }
            
            var venue = JsonSerializer.Deserialize<Venue>(content, _jsonSerializerOptions);
            return venue;
        }

        public async Task<Venue?> PostVenueAsync(Venue item)
        {
            var endpoint = $"api/venues";
            var token = await _backOfficeServerTokenClient.GetTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PostAsJsonAsync(endpoint, item);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(content))
            {
                return null;
            }

            var created = JsonSerializer.Deserialize<Venue>(content, _jsonSerializerOptions);
            return created;
        }

        public async Task DeleteVenueAsync(Venue item)
        {
            var endpoint = $"api/venues/{item.Id}";
            var token = await _backOfficeServerTokenClient.GetTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.DeleteAsync(endpoint);
            response.EnsureSuccessStatusCode();
        }
    }
}
