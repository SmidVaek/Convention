namespace Conventions.Models
{
    public class EventReservation
    {
        public string? Id { get; set; }
        public string EventId { get; set; } = null!;
        public string UserId { get; set; } = null!;
    }
}
