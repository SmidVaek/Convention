using Conventions.Models;
using Conventions.Storage.Mongo.Models;
using MongoDB.Bson;

namespace Conventions.Storage.Mongo.Extensions
{
    internal static class VenueExtensions
    {
        public static DbVenue ToDb(this Venue venue)
        {
            return new DbVenue()
            {
                Id = string.IsNullOrEmpty(venue.Id) ? default : ObjectId.Parse(venue.Id),
                Address = venue.Address,
                Name = venue.Name,
            };
        }

        public static Venue FromDb(this DbVenue venue)
        {
            return new Venue()
            {
                Id = venue.Id.ToString(),
                Address = venue.Address,
                Name = venue.Name,
            };
        }
    }
}
