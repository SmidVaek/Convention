using Conventions.Models;

namespace Conventions.Storage
{
    public interface IVenueStore
    {
        Task<Venue> AddAsync(Venue venue);
        Task RemoveAsync(Venue venue);
        Task<IEnumerable<Venue>> GetAllAsync();
        Task<Venue?> FindByNameAsync(string name);
        Task<Venue?> FindByIdAsync(string id);
    }
}