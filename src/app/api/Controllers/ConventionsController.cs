using Conventions.Models;
using Conventions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace conventions.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "s2s")]
    public class ConventionsController : ControllerBase
    {
        private readonly IConventionService _conventionService;
        private readonly IAssociationService _associationService;

        public ConventionsController(IConventionService conventionService,
                                    IAssociationService associationService)
        {
            _conventionService = conventionService;
            _associationService = associationService;
        }

        #region Public
        [AllowAnonymous]
        [HttpGet()]
        public async Task<IActionResult> GetConventions()
        {
            var conventions = await _conventionService.GetAllAsync();
            return new OkObjectResult(conventions);
        }

        [AllowAnonymous]
        [HttpGet("{conventionId}")]
        public async Task<IActionResult> GetConventionById(string conventionId)
        {
            var convention = await _conventionService.GetByIdAsync(conventionId);
            return new OkObjectResult(convention);
        }

        [AllowAnonymous]
        [HttpGet("{conventionId}/venues")]
        public async Task<IActionResult> GetConventionVenues(string conventionId)
        {
            var associations = await _associationService.GetConventionAssociatedVenues(conventionId);
            return new OkObjectResult(associations);
        }

        [AllowAnonymous]
        [HttpGet("{conventionId}/venues/{venueId}")]
        public async Task<IActionResult> GetConventionVenueDetails(string conventionId, string venueId)
        {
            var association = await _associationService.GetConventionAssociatedVenues(conventionId);
            var found = association.Any(x => x.Id == venueId);
            return new OkObjectResult(found);
        }

        [AllowAnonymous]
        [HttpGet("{conventionId}/events")]
        public async Task<IActionResult> GetConventionEvents(string conventionId)
        {
            var associations = await _associationService.GetConventionAssociatedEvents(conventionId);
            return new OkObjectResult(associations);
        }

        [AllowAnonymous]
        [HttpGet("{conventionId}/events/{eventId}")]
        public async Task<IActionResult> GetConventionEventDetails(string conventionId, string eventId)
        {
            var association = await _associationService.GetConventionAssociatedEvents(conventionId);
            var found = association.Any(x => x.Id == eventId);
            return new OkObjectResult(found);
        }
        #endregion

        #region Auth
        #region Conventions
        [HttpPost("")]
        public async Task<IActionResult> AddConvention(Convention convention)
        {
            var updatedConvention = await _conventionService.CreateAsync(convention);
            return new CreatedResult($"/{updatedConvention.Id}", updatedConvention);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConvention(string id)
        {
            await _conventionService.DeleteAsync(id);
            return Ok();
        }
        #endregion

        #region Associate Venue
        [HttpPost("{conventionId}/venues/{venueId}")]
        public async Task<IActionResult> AssociateVenue(string conventionId, string venueId)
        {
            await _associationService.AssociateVenue(conventionId, venueId);
            return Ok();
        }

        [HttpDelete("{conventionId}/venues/{venueId}")]
        public async Task<IActionResult> DeassociateVenue(string conventionId, string venueId)
        {
            await _associationService.DisassociateVenue(conventionId, venueId);
            return Ok();
        }
        #endregion

        #region Associate Event
        [HttpPost("{conventionId}/events/{eventId}")]
        public async Task<IActionResult> AssociateEvent(string conventionId, string eventId)
        {
            await _associationService.AssociateEvent(conventionId, eventId);
            return Ok();
        }

        [HttpDelete("{conventionId}/events/{eventId}")]
        public async Task<IActionResult> DeassociateConventionWithEvent(string conventionId, string eventId)
        {
            await _associationService.DisassociateEvent(conventionId, eventId);
            return Ok();
        }
        #endregion

        #region Register for Event
        [HttpPost("{conventionId}/events/{eventId}/register")]
        [Authorize(Policy = "oidc")]
        public async Task<IActionResult> RegisterForEvent(string conventionId, string eventId)
        {
            var userId = User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
            await _associationService.RegisterForEvent(conventionId, eventId, userId);
            return Ok();
        }

        [HttpDelete("{conventionId}/events/{eventId}/register")]
        [Authorize(Policy = "oidc")]
        public async Task<IActionResult> UnregisterForEvent(string conventionId, string eventId)
        {
            var userId = User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
            await _associationService.UnregisterForEvent(conventionId, eventId, userId);
            return Ok();
        }
        #endregion
        #endregion
    }
}
