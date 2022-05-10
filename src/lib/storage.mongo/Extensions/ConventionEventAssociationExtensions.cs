using Conventions.Models;
using Conventions.Storage.Mongo.Models;
using MongoDB.Bson;

namespace Conventions.Storage.Mongo.Extensions
{
    internal static class DbConventionEventAssociationExtensions
    {
        public static DbConventionEventAssociation ToDb(this ConventionEventAssociation association)
        {
            return new DbConventionEventAssociation()
            {
                Id = string.IsNullOrEmpty(association.Id) ? default : ObjectId.Parse(association.Id),
                ConventionId = ObjectId.Parse(association.ConventionId),
                EventId = ObjectId.Parse(association.EventId),
            };
        }

        public static ConventionEventAssociation FromDb(this DbConventionEventAssociation association)
        {
            return new ConventionEventAssociation()
            {
                Id = association.Id.ToString(),
                ConventionId = association.ConventionId.ToString(),
                EventId = association.EventId.ToString(),
            };
        }
    }
}
