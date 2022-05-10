namespace Conventions.Models
{
    public class Event
    {
        public string? Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTimeOffset Start { get; set; }
        public DateTimeOffset End { get; set; }
    }
}
