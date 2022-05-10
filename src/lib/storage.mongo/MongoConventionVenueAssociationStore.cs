using Conventions.Models;
using Conventions.Storage.Mongo.Extensions;
using Conventions.Storage.Mongo.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Conventions.Storage.Mongo
{
    public class MongoConventionVenueAssociationStore : MongoBaseStore<DbConventionVenueAssociation>, IConventionVenueAssociationStore
    {
        public MongoConventionVenueAssociationStore(IOptions<MongoConventionVenueAssociationStoreOptions> options) : base(options) { }

        public async Task<ConventionVenueAssociation> AddAsync(ConventionVenueAssociation item)
        {
            var dbAssociation = item.ToDb();
            await _collection.InsertOneAsync(dbAssociation);
            return dbAssociation.FromDb();
        }

        public async Task DeleteAsync(ConventionVenueAssociation item)
        {
            var objectId = ObjectId.Parse(item.Id);
            await _collection.DeleteOneAsync(x => x.Id == objectId);
        }

        public async Task DeleteByEventIdAsync(ConventionVenueAssociation item)
        {
            var objectId = ObjectId.Parse(item.VenueId);
            await _collection.DeleteManyAsync(x => x.VenueId == objectId);
        }

        public async Task DeleteByConventionIdAsync(ConventionVenueAssociation item)
        {
            var objectId = ObjectId.Parse(item.ConventionId);
            await _collection.DeleteManyAsync(x => x.ConventionId == objectId);
        }

        public async Task<IEnumerable<ConventionVenueAssociation>> GetByConventionIdAsync(string conventionId)
        {
            var objectId = ObjectId.Parse(conventionId);
            var matches = await _collection.FindAsync(x => x.ConventionId == objectId);
            return matches.ToEnumerable().Select(x => x.FromDb());
        }

        public async Task<IEnumerable<ConventionVenueAssociation>> GetByVenueIdAsync(string venueId)
        {
            var objectId = ObjectId.Parse(venueId);
            var matches = await _collection.FindAsync(x => x.VenueId == objectId);
            return matches.ToEnumerable().Select(x => x.FromDb());
        }

        public async Task<ConventionVenueAssociation?> GetByIdAsync(string associationId)
        {
            var objectId = ObjectId.Parse(associationId);
            var matches = await _collection.FindAsync(x => x.Id == objectId);
            var match = matches.FirstOrDefault();
            return match.FromDb() ?? null;
        }

        public async Task<ConventionVenueAssociation?> GetByConventionIdAndVenueIdAsync(string conventionId, string venueId)
        {
            var conventionObjectId = ObjectId.Parse(conventionId);
            var venueObjectId = ObjectId.Parse(venueId);
            var matches = await _collection.FindAsync(x => x.ConventionId == conventionObjectId && x.VenueId == venueObjectId);
            var match = matches.FirstOrDefault();
            return match.FromDb() ?? null;
        }
    }
}
