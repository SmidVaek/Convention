using Conventions.Models;

namespace Conventions.Storage
{
    public interface IEventStore
    {
        Task<Event> AddAsync(Event e);
        Task RemoveAsync(Event e);
        Task UpdateAsync(Event e);
        Task<IEnumerable<Event>> GetAllAsync();
        Task<Event?> FindByNameAsync(string name);
        Task<Event?> FindByIdAsync(string id);
    }
}