using Conventions.Models;
using Conventions.Storage.Mongo.Extensions;
using Conventions.Storage.Mongo.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Conventions.Storage.Mongo
{
    public class MongoEventReservationStore : MongoBaseStore<DbEventReservation>, IEventRegistrationStore
    {
        public MongoEventReservationStore(IOptions<MongoEventReservationStoreOptions> options) : base(options) { }

        public async Task<EventReservation> AddAsync(EventReservation item)
        {
            var dbConventionEventAssociation = item.ToDb();
            await _collection.InsertOneAsync(dbConventionEventAssociation);
            return dbConventionEventAssociation.FromDb();
        }

        public async Task DeleteAsync(EventReservation item)
        {
            var objectId = ObjectId.Parse(item.Id);
            await _collection.DeleteOneAsync(x => x.Id == objectId);
        }

        public async Task DeleteByEventIdAsync(string eventId)
        {
            var objectId = ObjectId.Parse(eventId);
            await _collection.DeleteManyAsync(x => x.EventId == objectId);
        }

        public async Task DeleteByUserIdAsync(string userId)
        {
            var objectId = ObjectId.Parse(userId);
            await _collection.DeleteManyAsync(x => x.UserId == objectId);
        }

        public async Task<IEnumerable<EventReservation>> GetByUserIdAsync(string userId)
        {
            var objectId = ObjectId.Parse(userId);
            var matches = await _collection.FindAsync(x => x.UserId == objectId);
            return matches.ToEnumerable().Select(x => x.FromDb());
        }

        public async Task<IEnumerable<EventReservation>> GetByEventIdAsync(string eventId)
        {
            var objectId = ObjectId.Parse(eventId);
            var matches = await _collection.FindAsync(x => x.EventId == objectId);
            return matches.ToEnumerable().Select(x => x.FromDb());
        }

        public async Task<EventReservation?> GetByIdAsync(string associationId)
        {
            var objectId = ObjectId.Parse(associationId);
            var matches = await _collection.FindAsync(x => x.Id == objectId);
            var match = matches.FirstOrDefault();
            return match.FromDb() ?? null;
        }

        public async Task<EventReservation?> GetByEventIdAndUserIdAsync(string eventId, string userId)
        {
            var eventObjectId = ObjectId.Parse(eventId);
            var userObjectId = ObjectId.Parse(userId);
            var matches = await _collection.FindAsync(x => x.EventId == eventObjectId && x.UserId == userObjectId);
            var match = matches.FirstOrDefault();
            return match.FromDb() ?? null;
        }
    }
}
