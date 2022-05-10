using Conventions.Models;
using Conventions.Storage.Mongo.Extensions;
using Conventions.Storage.Mongo.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Conventions.Storage.Mongo
{
    public class MongoConventionEventAssociationStore : MongoBaseStore<DbConventionEventAssociation>, IConventionEventAssociationStore
    {

        public MongoConventionEventAssociationStore(IOptions<MongoConventionEventAssociationStoreOptions> options) : base(options) { }

        public async Task<ConventionEventAssociation> AddAsync(ConventionEventAssociation item)
        {
            var dbConventionEventAssociation = item.ToDb();
            await _collection.InsertOneAsync(dbConventionEventAssociation);
            return dbConventionEventAssociation.FromDb();
        }

        public async Task DeleteAsync(ConventionEventAssociation item)
        {
            var objectId = ObjectId.Parse(item.Id);
            await _collection.DeleteOneAsync(x => x.Id == objectId);
        }

        public async Task DeleteByEventIdAsync(ConventionEventAssociation item)
        {
            var objectId = ObjectId.Parse(item.EventId);
            await _collection.DeleteManyAsync(x => x.EventId == objectId);
        }

        public async Task DeleteByConventionIdAsync(ConventionEventAssociation item)
        {
            var objectId = ObjectId.Parse(item.ConventionId);
            await _collection.DeleteManyAsync(x => x.ConventionId == objectId);
        }

        public async Task<IEnumerable<ConventionEventAssociation>> GetByConventionIdAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            var matches = await _collection.FindAsync(x => x.ConventionId == objectId);
            return matches.ToEnumerable().Select(x => x.FromDb());
        }

        public async Task<IEnumerable<ConventionEventAssociation>> GetByEventIdAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            var matches = await _collection.FindAsync(x => x.EventId == objectId);
            return matches.ToEnumerable().Select(x => x.FromDb());
        }

        public async Task<ConventionEventAssociation?> GetByIdAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            var matches = await _collection.FindAsync(x => x.Id == objectId);
            var match = matches.FirstOrDefault();
            return match.FromDb() ?? null;
        }

        public async Task<ConventionEventAssociation?> GetByConventionIdAndEventIdAsync(string conventionId, string eventId)
        {
            var conventionObjectId = ObjectId.Parse(conventionId);
            var eventObjectId = ObjectId.Parse(eventId);
            var matches = await _collection.FindAsync(x => x.ConventionId == conventionObjectId && x.EventId == eventObjectId);
            var match = matches.FirstOrDefault();
            return match.FromDb() ?? null;
        }
    }
}
