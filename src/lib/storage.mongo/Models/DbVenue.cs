using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Conventions.Storage.Mongo.Models
{
    public class DbVenue
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
    }
}
