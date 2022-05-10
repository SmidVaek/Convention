using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Conventions.Storage.Mongo.Models
{
    public class DbConventionVenueAssociation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId ConventionId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId VenueId { get; set; }
    }
}
