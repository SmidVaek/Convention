using Conventions.Models;
using Conventions.Storage.Mongo.Models;
using MongoDB.Bson;

namespace Conventions.Storage.Mongo.Extensions
{
    internal static class EventExtensions
    {
        public static DbEvent ToDb(this Event e)
        {
            var dbEvent = new DbEvent()
            {
                Id = string.IsNullOrEmpty(e.Id) ? default : ObjectId.Parse(e.Id),
                Name = e.Name,
                Description = e.Description,
                End = e.End,
                Start = e.Start,
            };

            return dbEvent;
        }

        public static Event FromDb(this DbEvent e)
        {
            return new Event()
            {
                Id = e.Id.ToString(),
                Name = e.Name,
                Description = e.Description,
                End = e.End,
                Start = e.Start,
            };
        }
    }
}
