using Conventions.Models;
using Conventions.Storage.Mongo.Extensions;
using Conventions.Storage.Mongo.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Conventions.Storage.Mongo
{
    public class MongoEventStore : MongoBaseStore<DbEvent>, IEventStore
    {
        public MongoEventStore(IOptions<MongoEventStoreOptions> options) : base(options) { }

        public async Task<Event> AddAsync(Event e)
        {
            var dbEvent = e.ToDb();
            await _collection.InsertOneAsync(dbEvent);
            return dbEvent.FromDb();
        }

        public async Task RemoveAsync(Event e)
        {
            var objectId = ObjectId.Parse(e.Id);
            await _collection.DeleteOneAsync(x => x.Id == objectId);
        }

        public async Task UpdateAsync(Event e)
        {
            var objectId = ObjectId.Parse(e.Id);
            var dbEvent = e.ToDb();
            await _collection.ReplaceOneAsync(x => x.Id == objectId, dbEvent);
        }

        public async Task<IEnumerable<Event>> GetAllAsync()
        {
            var all = await _collection.FindAsync(x => true);
            var events = all.ToEnumerable().Select(e => e.FromDb()).ToList();
            return events;
        }

        public async Task<Event?> FindByNameAsync(string name)
        {
            var events = await _collection.FindAsync(x => x.Name == name);
            var e = events.FirstOrDefault();

            if (e == default)
            {
                return null;
            }

            return e.FromDb();
        }

        public async Task<Event?> FindByIdAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            var events = await _collection.FindAsync(x => x.Id == objectId);
            var e = events.FirstOrDefault();

            if (e == default)
            {
                return null;
            }

            return e.FromDb();
        }
    }
}
