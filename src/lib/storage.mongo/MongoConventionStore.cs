using Conventions.Models;
using Conventions.Storage.Mongo.Extensions;
using Conventions.Storage.Mongo.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Conventions.Storage.Mongo
{
    public class MongoConventionStore : MongoBaseStore<DbConvention>, IConventionStore
    {
        public MongoConventionStore(IOptions<MongoConventionStoreOptions> options) : base(options) { }

        public async Task<Convention> AddAsync(Convention convention)
        {
            var dbConvention = convention.ToDb();
            await _collection.InsertOneAsync(dbConvention);
            return dbConvention.FromDb();
        }

        public async Task DeleteAsync(Convention convention)
        {
            var objectId = ObjectId.Parse(convention.Id);
            await _collection.DeleteOneAsync(x => x.Id == objectId);
        }

        public async Task UpdateAsync(Convention convention)
        {
            var objectId = ObjectId.Parse(convention.Id);
            var dbConvention = convention.ToDb();
            await _collection.ReplaceOneAsync(x => x.Id == objectId, dbConvention);
        }

        public async Task<IEnumerable<Convention>> GetAllAsync()
        {
            var allConventions = await _collection.FindAsync(x => true);
            var dbConventions = allConventions.ToEnumerable().Select(x => x.FromDb()).ToList();
            return dbConventions;
        }



        public async Task<Convention?> GetByNameAsync(string name)
        {
            var matchConventions = await _collection.FindAsync(x => x.Name == name);
            var dbConvention = matchConventions.FirstOrDefault();

            if (dbConvention == default)
            {
                return null;
            }

            var convention = dbConvention.FromDb();
            return convention;
        }

        public async Task<Convention?> GetByIdAsync(string id)
        {
            var objectId = ObjectId.Parse(id);

            var matchConventions = await _collection.FindAsync(x => x.Id == objectId);
            var dbConvention = matchConventions.FirstOrDefault();

            if (dbConvention == default)
            {
                return null;
            }

            var convention = dbConvention.FromDb();
            return convention;
        }
    }
}
