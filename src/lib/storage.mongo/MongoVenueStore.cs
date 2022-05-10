using Conventions.Models;
using Conventions.Storage.Mongo.Extensions;
using Conventions.Storage.Mongo.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Conventions.Storage.Mongo
{
    public class MongoVenueStore : MongoBaseStore<DbVenue>, IVenueStore
    {
        public MongoVenueStore(IOptions<MongoVenueStoreOptions> options) : base(options) { }

        public async Task<Venue> AddAsync(Venue venue)
        {
            var dbVenue = venue.ToDb();
            await _collection.InsertOneAsync(dbVenue);
            return dbVenue.FromDb();
        }

        public async Task UpdateAsync(Venue venue)
        {
            var objectId = ObjectId.Parse(venue.Id);
            var dbVenue = venue.ToDb();
            _ = await _collection.ReplaceOneAsync(x => x.Id == objectId, dbVenue);
        }

        public async Task<Venue?> FindByIdAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            var matches = await _collection.FindAsync(x => x.Id == objectId);
            var venue = matches.FirstOrDefault();

            if (venue == default)
            {
                return null;
            }

            return venue.FromDb();
        }

        public async Task<Venue?> FindByNameAsync(string name)
        {
            var matches = await _collection.FindAsync(x => x.Name == name);
            var venue = matches.FirstOrDefault();

            if (venue == default)
            {
                return null;
            }

            return venue.FromDb();
        }

        public async Task<IEnumerable<Venue>> GetAllAsync()
        {
            var matches = await _collection.FindAsync(x => true);
            var venues = matches.ToEnumerable().Select(x => x.FromDb()).ToList();
            return venues;
        }

        public async Task RemoveAsync(Venue venue)
        {
            var objectId = ObjectId.Parse(venue.Id);
            await _collection.DeleteOneAsync(x => x.Id == objectId);
        }
    }
}
