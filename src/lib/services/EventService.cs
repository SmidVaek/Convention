using Conventions.Models;
using Conventions.Storage;

namespace Conventions.Services
{
    public interface IEventService
    {
        Task<IEnumerable<Event>> GetAllAsync();
        Task<Event?> GetByIdAsync(string id);
        Task DeleteAsync(string id);
        Task<Event> CreateAsync(Event e);
        Task UpdateAsync(Event e);
    }
    public class EventService: IEventService
    {
        private readonly IEventStore _eventStore;

        public EventService(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public async Task<IEnumerable<Event>> GetAllAsync()
        {
            var events = await _eventStore.GetAllAsync();
            return events;
        }

        public async Task<Event?> GetByIdAsync(string id)
        {
            return await _eventStore.FindByIdAsync(id);
        }

        public async Task DeleteAsync(string id)
        {
            var e = await _eventStore.FindByIdAsync(id);

            if (e is null)
            {
                // throw some reasonable here?
                return;
            }

            await _eventStore.RemoveAsync(e);
        }

        public async Task<Event> CreateAsync(Event e)
        {
            return await _eventStore.AddAsync(e);
        }

        public async Task UpdateAsync(Event e)
        {
            await _eventStore.UpdateAsync(e);
        }
    }
}