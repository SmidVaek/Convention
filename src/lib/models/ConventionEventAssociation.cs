namespace Conventions.Models
{
    public class ConventionEventAssociation
    {
        public string? Id { get; set; }
        public string ConventionId { get; set; } = null!;
        public string EventId { get; set; } = null!;
    }
}
