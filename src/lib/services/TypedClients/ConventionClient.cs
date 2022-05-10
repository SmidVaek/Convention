using Conventions.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace Conventions.Services.TypedClients
{
    public class ConventionClient
    {
        private readonly HttpClient _httpClient;
        private readonly BackOfficeServerTokenClient _backOfficeServerTokenClient;
        public ConventionClient(HttpClient httpClient,
                                BackOfficeServerTokenClient backOfficeServerTokenClient)
        {
            _httpClient = httpClient;
            _backOfficeServerTokenClient = backOfficeServerTokenClient;
        }

        #region Public
        public async Task<IEnumerable<Convention>> GetConventions()
        {
            var endpoint = "api/conventions";
            var responseObject = await _httpClient.GetFromJsonAsync<Convention[]>(endpoint);
            return responseObject;
        }

        public async Task<Convention?> GetConventionById(string conventionId)
        {
            var endpoint = $"api/conventions/{conventionId}";
            var responseObject = await _httpClient.GetFromJsonAsync<Convention?>(endpoint);
            return responseObject;
        }

        public async Task<IEnumerable<Venue>> GetConventionVenues(string conventionId)
        {
            var endpoint = $"api/conventions/{conventionId}/venues";
            var responseObject = await _httpClient.GetFromJsonAsync<Venue[]>(endpoint);
            return responseObject;
        }

        public async Task<Venue?> GetConventionVenueDetails(string conventionId, string venueId)
        {
            var endpoint = $"api/conventions/{conventionId}/venues/{venueId}";
            var responseObject = await _httpClient.GetFromJsonAsync<Venue?>(endpoint);
            return responseObject;
        }

        public async Task<IEnumerable<Event>> GetConventionEvents(string conventionId)
        {
            var endpoint = $"api/conventions/{conventionId}/events";
            var responseObject = await _httpClient.GetFromJsonAsync<Event[]>(endpoint);
            return responseObject;
        }

        public async Task<Event?> GetConventionEventDetails(string conventionId, string eventId)
        {
            var endpoint = $"api/conventions/{conventionId}/events/{eventId}";
            var responseObject = await _httpClient.GetFromJsonAsync<Event?>(endpoint);
            return responseObject;
        }
        #endregion

        #region Auth
        public async Task<Convention> AddConvention(Convention convention)
        {
            var endpoint = $"api/conventions";
            var httpContent = JsonContent.Create(convention); 
            var response = await PostAuthenticated(endpoint, httpContent);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<Convention>(content);
            return responseObject;
        }

        public async Task DeleteConvention(string id)
        {
            var endpoint = $"api/conventions/{id}";
            var response = await DeleteAuthenticated(endpoint);
            response.EnsureSuccessStatusCode();
        }

        #region Associate Venue
        public async Task AssociateVenue(string conventionId, string venueId)
        {
            var endpoint = $"api/conventions/{conventionId}/venues/{venueId}";
            var response = await PostAuthenticated(endpoint);
            response.EnsureSuccessStatusCode();
        }

        public async Task DisassociateVenue(string conventionId, string venueId)
        {
            var endpoint = $"api/conventions/{conventionId}/venues/{venueId}";
            var response = await DeleteAuthenticated(endpoint);
            response.EnsureSuccessStatusCode();
        }
        #endregion

        #region Associate Event
        public async Task AssociateEvent(string conventionId, string eventId)
        {
            var endpoint = $"api/conventions/{conventionId}/events/{eventId}";
            var response = await PostAuthenticated(endpoint);
            response.EnsureSuccessStatusCode();
        }

        public async Task DisassociateEvent(string conventionId, string eventId)
        {
            var endpoint = $"api/conventions/{conventionId}/events/{eventId}";
            var response = await DeleteAuthenticated(endpoint);
            response.EnsureSuccessStatusCode();
        }
        #endregion
        #endregion

        #region Helpers
        private async Task<HttpResponseMessage> PostAuthenticated(string endpoint, HttpContent payload = null)
        {
            var token = await _backOfficeServerTokenClient.GetTokenAsync();
            using var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            request.Content = payload;
            return await _httpClient.SendAsync(request);
        }

        private async Task<HttpResponseMessage> DeleteAuthenticated(string endpoint)
        {
            var token = await _backOfficeServerTokenClient.GetTokenAsync();
            using var request = new HttpRequestMessage(HttpMethod.Delete, endpoint);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            return await _httpClient.SendAsync(request);
        }
        #endregion
    }
}
