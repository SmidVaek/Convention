using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Conventions.Storage.Mongo.Models
{
    public class DbEventReservation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId EventId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId UserId { get; set; }
    }
}
