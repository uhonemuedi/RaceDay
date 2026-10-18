using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using RaceDay.Api.Helpers;

namespace RaceDay.Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class EnrolmentsController : ControllerBase
    {
        private readonly RaceDayContext _context;

        public EnrolmentsController(RaceDayContext context)
        {
            _context = context;
        }

        public class EnrolDto
        {
            public int CategoryID { get; set; }
        }

        // POST /api/enrolments — Participant only
        [HttpPost("enrolments")]
        public async Task<IActionResult> Enrol(EnrolDto dto)
        {
            if (!SessionHelper.IsAuthenticated(HttpContext))
                return Unauthorized("You must be logged in.");

            if (SessionHelper.GetRole(HttpContext) != "Participant")
                return Forbid();

            var categoryExists = await _context.Categories.AnyAsync(c => c.CategoryID == dto.CategoryID);
            if (!categoryExists) return NotFound("Category not found.");

            var participantId = SessionHelper.GetUserId(HttpContext)!.Value;

            var alreadyEnrolled = await _context.Enrolments.AnyAsync(e =>
                e.ParticipantID == participantId && e.CategoryID == dto.CategoryID);

            if (alreadyEnrolled)
                return Conflict("Already enrolled in this category.");

            var enrolment = new Enrolment
            {
                ParticipantID = participantId,
                CategoryID = dto.CategoryID,
                Status = "Confirmed"
            };

            _context.Enrolments.Add(enrolment);
            await _context.SaveChangesAsync();

            return StatusCode(201, enrolment);
        }

        // GET /api/enrolments/me — Participant only, own enrolments
        [HttpGet("enrolments/me")]
        public async Task<IActionResult> GetMyEnrolments()
        {
            if (!SessionHelper.IsAuthenticated(HttpContext))
                return Unauthorized("You must be logged in.");

            if (SessionHelper.GetRole(HttpContext) != "Participant")
                return Forbid();

            var participantId = SessionHelper.GetUserId(HttpContext)!.Value;

            var enrolments = await _context.Enrolments
                .Include(e => e.Category)
                .Where(e => e.ParticipantID == participantId)
                .ToListAsync();

            return Ok(enrolments);
        }

        // GET /api/events/{eventId}/enrolments — Organiser only, must own the event
        [HttpGet("events/{eventId}/enrolments")]
        public async Task<IActionResult> GetEnrolmentsForEvent(int eventId)
        {
            if (!SessionHelper.IsAuthenticated(HttpContext))
                return Unauthorized("You must be logged in.");

            if (SessionHelper.GetRole(HttpContext) != "Organiser")
                return Forbid();

            var ev = await _context.Events.FindAsync(eventId);
            if (ev == null) return NotFound("Event not found.");

            var organiserId = SessionHelper.GetUserId(HttpContext)!.Value;
            if (ev.OrganiserID != organiserId)
                return Forbid();

            var enrolments = await _context.Enrolments
                .Include(e => e.Participant)
                .Include(e => e.Category)
                .Where(e => e.Category!.EventID == eventId)
                .ToListAsync();

            return Ok(enrolments);
        }

        // DELETE /api/enrolments/{id} — Participant only, own enrolment
        [HttpDelete("enrolments/{id}")]
        public async Task<IActionResult> CancelEnrolment(int id)
        {
            if (!SessionHelper.IsAuthenticated(HttpContext))
                return Unauthorized("You must be logged in.");

            if (SessionHelper.GetRole(HttpContext) != "Participant")
                return Forbid();

            var enrolment = await _context.Enrolments.FindAsync(id);
            if (enrolment == null) return NotFound();

            var participantId = SessionHelper.GetUserId(HttpContext)!.Value;
            if (enrolment.ParticipantID != participantId)
                return Forbid();

            _context.Enrolments.Remove(enrolment);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
