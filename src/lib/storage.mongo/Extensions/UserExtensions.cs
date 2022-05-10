using Conventions.Models;
using Conventions.Storage.Mongo.Models;
using MongoDB.Bson;

namespace Conventions.Storage.Mongo.Extensions
{
    internal static class UserExtensions
    {
        public static DbUser ToDb(this User user)
        {
            return new DbUser()
            {
                Id = string.IsNullOrEmpty(user.Id) ? default : ObjectId.Parse(user.Id),
                Name = user.Name,
                Address = user.Address,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Password = user.Password,
                Roles = user.Roles ?? Array.Empty<string>(),
            };
        }

        public static User FromDb(this DbUser user)
        {
            return new User()
            {
                Id = user.Id.ToString(),
                Name = user.Name,
                Address = user.Address,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Password = user.Password,
                Roles = user.Roles,
            };
        }
    }
}
