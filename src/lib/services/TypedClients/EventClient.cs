using Conventions.Models;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Conventions.Services.TypedClients
{
    public class EventClient
    {
        private readonly HttpClient _httpClient;
        private readonly BackOfficeServerTokenClient _backOfficeServerTokenClient;
        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        public EventClient(HttpClient httpClient,
                           BackOfficeServerTokenClient backOfficeServerTokenClient)
        {
            _httpClient = httpClient;
            _backOfficeServerTokenClient = backOfficeServerTokenClient;
        }

        public async Task<IEnumerable<Event>?> GetEventsAsync()
        {
            var endpoint = "api/events";
            var token = await _backOfficeServerTokenClient.GetTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(content))
            {
                return Array.Empty<Event>();
            }

            var venues = JsonSerializer.Deserialize<List<Event>>(content, _jsonSerializerOptions);
            return venues;
        }

        public async Task<Event?> GetEventAsync(string id)
        {
            var endpoint = $"api/events/{id}";
            var token = await _backOfficeServerTokenClient.GetTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(content))
            {
                return null;
            }
            
            var venue = JsonSerializer.Deserialize<Event>(content, _jsonSerializerOptions);
            return venue;
        }

        public async Task<Event?> PostEventAsync(Event e)
        {
            var endpoint = $"api/events";
            var token = await _backOfficeServerTokenClient.GetTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PostAsJsonAsync(endpoint, e);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(content))
            {
                return null;
            }

            var created = JsonSerializer.Deserialize<Event>(content, _jsonSerializerOptions);
            return created;
        }

        public async Task<Event?> PutEventAsync(Event e)
        {
            var endpoint = $"api/events";
            var token = await _backOfficeServerTokenClient.GetTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PutAsJsonAsync(endpoint, e);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(content))
            {
                return null;
            }

            var created = JsonSerializer.Deserialize<Event>(content, _jsonSerializerOptions);
            return created;
        }

        public async Task DeleteEventAsync(Event e)
        {
            var endpoint = $"api/events/{e.Id}";
            var token = await _backOfficeServerTokenClient.GetTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.DeleteAsync(endpoint);
            response.EnsureSuccessStatusCode();
        }
    }
}
