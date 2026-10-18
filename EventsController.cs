using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using RaceDay.Api.Helpers;

namespace RaceDay.Api.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventsController : ControllerBase
    {
        private readonly RaceDayContext _context;

        public EventsController(RaceDayContext context)
        {
            _context = context;
        }

        public class EventDto
        {
            public string EventName { get; set; }
            public DateTime EventDate { get; set; }
            public string? Description { get; set; }
            public string EventType { get; set; }
            public int VenueID { get; set; }
        }

        // GET /api/events — anyone can view
        [HttpGet]
        public async Task<IActionResult> GetEvents()
        {
            var events = await _context.Events
                .Include(e => e.Venue)
                .Include(e => e.Categories)
                .ToListAsync();
            return Ok(events);
        }

        // GET /api/events/{id} — anyone can view
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEvent(int id)
        {
            var ev = await _context.Events
                .Include(e => e.Venue)
                .Include(e => e.Categories)
                .FirstOrDefaultAsync(e => e.EventID == id);

            if (ev == null) return NotFound();
            return Ok(ev);
        }

        // POST /api/events — Organiser only
        [HttpPost]
        public async Task<IActionResult> CreateEvent(EventDto dto)
        {
            if (!SessionHelper.IsAuthenticated(HttpContext))
                return Unauthorized("You must be logged in.");

            if (SessionHelper.GetRole(HttpContext) != "Organiser")
                return Forbid();

            var newEvent = new Event
            {
                OrganiserID = SessionHelper.GetUserId(HttpContext)!.Value,
                VenueID = dto.VenueID,
                EventName = dto.EventName,
                EventDate = dto.EventDate,
                Description = dto.Description,
                EventType = dto.EventType
            };

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();

            return StatusCode(201, newEvent);
        }

        // PUT /api/events/{id} — Organiser only, must own the event
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(int id, EventDto dto)
        {
            if (!SessionHelper.IsAuthenticated(HttpContext))
                return Unauthorized("You must be logged in.");

            if (SessionHelper.GetRole(HttpContext) != "Organiser")
                return Forbid();

            var ev = await _context.Events.FindAsync(id);
            if (ev == null) return NotFound();

            if (ev.OrganiserID != SessionHelper.GetUserId(HttpContext))
                return Forbid();

            ev.EventName = dto.EventName;
            ev.EventDate = dto.EventDate;
            ev.Description = dto.Description;
            ev.EventType = dto.EventType;
            ev.VenueID = dto.VenueID;

            await _context.SaveChangesAsync();
            return Ok(ev);
        }

        // DELETE /api/events/{id} — Organiser only, must own the event
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            if (!SessionHelper.IsAuthenticated(HttpContext))
                return Unauthorized("You must be logged in.");

            if (SessionHelper.GetRole(HttpContext) != "Organiser")
                return Forbid();

            var ev = await _context.Events.FindAsync(id);
            if (ev == null) return NotFound();

            if (ev.OrganiserID != SessionHelper.GetUserId(HttpContext))
                return Forbid();

            _context.Events.Remove(ev);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
