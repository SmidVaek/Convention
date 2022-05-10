using Conventions.Models;

namespace Conventions.Storage
{
    public interface IConventionEventAssociationStore
    {
        Task<ConventionEventAssociation> AddAsync(ConventionEventAssociation item);
        Task DeleteAsync(ConventionEventAssociation item);
        Task DeleteByEventIdAsync(ConventionEventAssociation item);
        Task DeleteByConventionIdAsync(ConventionEventAssociation item);
        Task<IEnumerable<ConventionEventAssociation>> GetByEventIdAsync(string id);
        Task<IEnumerable<ConventionEventAssociation>> GetByConventionIdAsync(string id);
        Task<ConventionEventAssociation?> GetByIdAsync(string id);
        Task<ConventionEventAssociation?> GetByConventionIdAndEventIdAsync(string conventionId, string eventId);

    }
}
