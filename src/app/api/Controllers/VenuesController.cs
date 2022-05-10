using Conventions.Models;
using Conventions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace conventions.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "s2s")]
    public class VenuesController : ControllerBase
    {
        private readonly IVenueService _venueService;

        public VenuesController(IVenueService venueService)
        {
            _venueService = venueService;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetVenues()
        {
            var events = await _venueService.GetAllVenuesAsync();
            return new OkObjectResult(events);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVenue(string id)
        {
            var e = await _venueService.GetVenueAsync(id);
            return new OkObjectResult(e);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVenue(string id)
        {
            await _venueService.DeleteVenueAsync(id);
            return new OkResult();
        }

        [HttpPost("")]
        public async Task<IActionResult> CreateVenue(Venue venue)
        {
            var createdVenue = await _venueService.CreateVenueAsync(venue);
            return new CreatedResult($"/{createdVenue.Id}", createdVenue);
        }
    }
}
