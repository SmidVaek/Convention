using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Conventions.Storage.Mongo.Models
{
    public class DbEvent
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTimeOffset Start { get; set; }
        public DateTimeOffset End { get; set; }
    }
}
