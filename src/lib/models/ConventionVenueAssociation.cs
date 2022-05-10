namespace Conventions.Models
{
    public class ConventionVenueAssociation
    {
        public string? Id { get; set; }
        public string ConventionId { get; set; } = null!;
        public string VenueId { get; set; } = null!;
    }
}
