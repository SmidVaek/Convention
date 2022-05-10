using Conventions.Models;

namespace Conventions.Storage
{
    public interface IEventRegistrationStore
    {
        Task<EventReservation> AddAsync(EventReservation item);
        Task DeleteAsync(EventReservation item);
        Task DeleteByEventIdAsync(string eventId);
        Task DeleteByUserIdAsync(string userId);
        Task<IEnumerable<EventReservation>> GetByEventIdAsync(string eventId);
        Task<IEnumerable<EventReservation>> GetByUserIdAsync(string userId);
        Task<EventReservation?> GetByIdAsync(string registraionId);
        Task<EventReservation?> GetByEventIdAndUserIdAsync(string eventId, string userId);

    }
}
