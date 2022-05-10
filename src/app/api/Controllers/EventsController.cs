using Conventions.Models;
using Conventions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace conventions.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "s2s")]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet()]
        public async Task<IActionResult> GetEvents()
        {
            var events = await _eventService.GetAllAsync();
            return new OkObjectResult(events);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEvent(string id)
        {
            var e = await _eventService.GetByIdAsync(id);
            return new OkObjectResult(e);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(string id)
        {
            await _eventService.DeleteAsync(id);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent(Event e)
        {
            var createdEvent = await _eventService.CreateAsync(e);
            return new CreatedResult($"/{createdEvent.Id}", createdEvent);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateEventAsync(Event e)
        {
            await _eventService.UpdateAsync(e);
            return Ok();
        }
    }
}
