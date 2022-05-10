using Conventions.Models;
using Conventions.Storage;

namespace Conventions.Services
{
    public interface IVenueService
    {
        Task<IEnumerable<Venue>> GetAllVenuesAsync();
        Task<Venue?> GetVenueAsync(string id);
        Task DeleteVenueAsync(string id);
        Task<Venue> CreateVenueAsync(Venue venue);
    }

    public class VenueService: IVenueService
    {
        private readonly IVenueStore _venueStore;

        public VenueService(IVenueStore venueStore)
        {
            _venueStore = venueStore;
        }

        public async Task<IEnumerable<Venue>> GetAllVenuesAsync()
        {
            var venues = await _venueStore.GetAllAsync();
            return venues;
        }

        public async Task<Venue?> GetVenueAsync(string id)
        {
            var venue = await _venueStore.FindByIdAsync(id);
            return venue;
        }

        public async Task DeleteVenueAsync(string id)
        {
            var venue = await _venueStore.FindByIdAsync(id);
            if (venue is null)
            {
                // maybe throw something reasonable here
                return;
            }
            await _venueStore.RemoveAsync(venue);
        }

        public async Task<Venue> CreateVenueAsync(Venue venue)
        {
            var createdVenue = await _venueStore.AddAsync(venue);
            return createdVenue;    
        }
    }
}