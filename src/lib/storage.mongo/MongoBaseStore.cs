using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Conventions.Storage.Mongo
{
    public class MongoBaseStore<T>
    {
        private readonly IOptions<MongoBaseStoreOptions> _options;

        protected readonly MongoClient _mongoClient;
        protected readonly IMongoDatabase _database;
        protected readonly IMongoCollection<T> _collection;

        public MongoBaseStore(IOptions<MongoBaseStoreOptions> options)
        {
            _options = options;
            _mongoClient = new MongoClient(_options.Value.ConnectionString);
            _database = _mongoClient.GetDatabase(_options.Value.DatabaseName);

            var collectionExists = _database.ListCollectionNames().ToList().Any(x => x == options.Value.CollectionName);

            if (!collectionExists)
            {
                _database.CreateCollection(options.Value.CollectionName);
            }

            _collection = _database.GetCollection<T>(_options.Value.CollectionName);    
        }

        
    }
}