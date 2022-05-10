using Conventions.Models;
using Conventions.Storage.Mongo.Models;
using MongoDB.Bson;

namespace Conventions.Storage.Mongo.Extensions
{
    internal static class EventRegistrationExtensions
    {
        public static DbEventReservation ToDb(this EventReservation eventReservation)
        {
            return new DbEventReservation()
            {
                Id = string.IsNullOrEmpty(eventReservation.Id) ? default : ObjectId.Parse(eventReservation.Id),
                EventId = ObjectId.Parse(eventReservation.EventId),
                UserId = ObjectId.Parse(eventReservation.UserId),
            };
        }

        public static EventReservation FromDb(this DbEventReservation eventReservation)
        {
            return new EventReservation()
            {
                Id = eventReservation.Id.ToString(),
                EventId = eventReservation.EventId.ToString(),
                UserId = eventReservation.UserId.ToString(),
            };
        }
    }
}
