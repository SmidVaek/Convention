using Conventions.Models;
using Conventions.Storage.Mongo.Models;
using MongoDB.Bson;

namespace Conventions.Storage.Mongo.Extensions
{
    internal static class ConventionExtensions
    {
        public static DbConvention ToDb(this Convention convention)
        {
            return new DbConvention()
            {
                Name = convention.Name,
                Id = string.IsNullOrEmpty(convention.Id) ? default : ObjectId.Parse(convention.Id)

            };
        }

        public static Convention FromDb(this DbConvention convention)
        {
            return new Convention()
            {
                Id = convention.Id.ToString(),
                Name = convention.Name,
            };
        }
    }
}
