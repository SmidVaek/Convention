using Conventions.Models;
using Conventions.Storage.Mongo.Extensions;
using Conventions.Storage.Mongo.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Conventions.Storage.Mongo
{
    public class MongoUserStore : MongoBaseStore<DbUser>, IUserStore
    {
        public MongoUserStore(IOptions<MongoUserStoreOptions> options) : base(options) { }

        public async Task<User> AddAsync(User user)
        {
            var dbConvention = user.ToDb();
            await _collection.InsertOneAsync(dbConvention);
            return dbConvention.FromDb();
        }

        public async Task UpdateAsync(User user)
        {
            var objectId = ObjectId.Parse(user.Id);
            var dbUser = user.ToDb();
            _ = await _collection.ReplaceOneAsync(x => x.Id == objectId, dbUser);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            var matches = await _collection.FindAsync(x => x.Email == email);
            var dbUser = matches.FirstOrDefault();

            if (dbUser == default)
            {
                return null;
            }

            return dbUser.FromDb();
        }

        public async Task<User?> GetByIdAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            var matches = await _collection.FindAsync(x => x.Id == objectId);
            var dbUser = matches.FirstOrDefault();

            if (dbUser == default)
            {
                return null;
            }

            return dbUser.FromDb();
        }

        public async Task DeleteAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            _ = await _collection.DeleteOneAsync(x => x.Id == objectId);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            // implement skip by using cursor
            var matches = await _collection.FindAsync(x => true);
            return matches.ToEnumerable().Select(x => x.FromDb()).ToList();
        }

        public async Task<IEnumerable<User>> GetByRoleAsync(string role)
        {
            var matches = await _collection.FindAsync(x => x.Roles.Contains(role));
            var dbUsers = matches.ToEnumerable().Select(x => x.FromDb());
            return dbUsers.ToList();
        }
    }
}
