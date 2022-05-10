using Conventions.Models;

namespace Conventions.Storage
{
    public interface IConventionVenueAssociationStore
    {
        Task<ConventionVenueAssociation> AddAsync(ConventionVenueAssociation item);
        Task DeleteAsync(ConventionVenueAssociation item);
        Task DeleteByEventIdAsync(ConventionVenueAssociation item);
        Task DeleteByConventionIdAsync(ConventionVenueAssociation item);
        Task<IEnumerable<ConventionVenueAssociation>> GetByVenueIdAsync(string venueId);
        Task<IEnumerable<ConventionVenueAssociation>> GetByConventionIdAsync(string conventionId);
        Task<ConventionVenueAssociation?> GetByIdAsync(string associationId);
        Task<ConventionVenueAssociation?> GetByConventionIdAndVenueIdAsync(string conventionId, string venueId);

    }
}
