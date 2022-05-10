using Conventions.Models;
using Conventions.Storage.Mongo.Models;
using MongoDB.Bson;

namespace Conventions.Storage.Mongo.Extensions
{
    internal static class ConventionVenueAssociationExtensions
    {
        public static DbConventionVenueAssociation ToDb(this ConventionVenueAssociation association)
        {
            return new DbConventionVenueAssociation()
            {
                Id = string.IsNullOrEmpty(association.Id) ? default : ObjectId.Parse(association.Id),
                ConventionId = ObjectId.Parse(association.ConventionId),
                VenueId = ObjectId.Parse(association.VenueId),
            };
        }

        public static ConventionVenueAssociation FromDb(this DbConventionVenueAssociation association)
        {
            return new ConventionVenueAssociation()
            {
                Id = association.Id.ToString(),
                ConventionId = association.ConventionId.ToString(),
                VenueId = association.VenueId.ToString(),
            };
        }
    }
}
