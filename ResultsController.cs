using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using RaceDay.Api.Helpers;

namespace RaceDay.Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class ResultsController : ControllerBase
    {
        private readonly RaceDayContext _context;

        public ResultsController(RaceDayContext context)
        {
            _context = context;
        }

        public class ResultDto
        {
            public int EnrolmentID { get; set; }
            public TimeSpan? FinishTime { get; set; }
            public int? Position { get; set; }
            public string Status { get; set; } = "Finished";
        }

        public class ResultUpdateDto
        {
            public TimeSpan? FinishTime { get; set; }
            public int? Position { get; set; }
            public string Status { get; set; }
        }

        // POST /api/results — Organiser only, must own the event behind the enrolment
        [HttpPost("results")]
        public async Task<IActionResult> CaptureResult(ResultDto dto)
        {
            if (!SessionHelper.IsAuthenticated(HttpContext))
                return Unauthorized("You must be logged in.");

            if (SessionHelper.GetRole(HttpContext) != "Organiser")
                return Forbid();

            var enrolment = await _context.Enrolments
                .Include(e => e.Category)
                    .ThenInclude(c => c!.Event)
                .FirstOrDefaultAsync(e => e.EnrolmentID == dto.EnrolmentID);

            if (enrolment == null) return NotFound("Enrolment not found.");

            var organiserId = SessionHelper.GetUserId(HttpContext)!.Value;
            if (enrolment.Category!.Event!.OrganiserID != organiserId)
                return Forbid();

            var alreadyExists = await _context.Results.AnyAsync(r => r.EnrolmentID == dto.EnrolmentID);
            if (alreadyExists)
                return Conflict("Result already captured for this enrolment.");

            var result = new Result
            {
                EnrolmentID = dto.EnrolmentID,
                FinishTime = dto.FinishTime,
                Position = dto.Position,
                Status = dto.Status
            };

            _context.Results.Add(result);
            await _context.SaveChangesAsync();

            return StatusCode(201, result);
        }

        // GET /api/results/{enrolmentId} — anyone logged in can view
        [HttpGet("results/{enrolmentId}")]
        public async Task<IActionResult> GetResult(int enrolmentId)
        {
            if (!SessionHelper.IsAuthenticated(HttpContext))
                return Unauthorized("You must be logged in.");

            var result = await _context.Results
                .FirstOrDefaultAsync(r => r.EnrolmentID == enrolmentId);

            if (result == null) return NotFound();

            return Ok(result);
        }

        // GET /api/users/me/results — Participant only, own results
        [HttpGet("users/me/results")]
        public async Task<IActionResult> GetMyResults()
        {
            if (!SessionHelper.IsAuthenticated(HttpContext))
                return Unauthorized("You must be logged in.");

            if (SessionHelper.GetRole(HttpContext) != "Participant")
                return Forbid();

            var participantId = SessionHelper.GetUserId(HttpContext)!.Value;

            var results = await _context.Results
                .Include(r => r.Enrolment)
                    .ThenInclude(e => e!.Category)
                .Where(r => r.Enrolment!.ParticipantID == participantId)
                .ToListAsync();

            return Ok(results);
        }

        // PUT /api/results/{id} — Organiser only, must own the event behind the enrolment
        [HttpPut("results/{id}")]
        public async Task<IActionResult> UpdateResult(int id, ResultUpdateDto dto)
        {
            if (!SessionHelper.IsAuthenticated(HttpContext))
                return Unauthorized("You must be logged in.");

            if (SessionHelper.GetRole(HttpContext) != "Organiser")
                return Forbid();

            var result = await _context.Results
                .Include(r => r.Enrolment)
                    .ThenInclude(e => e!.Category)
                        .ThenInclude(c => c!.Event)
                .FirstOrDefaultAsync(r => r.ResultID == id);

            if (result == null) return NotFound();

            var organiserId = SessionHelper.GetUserId(HttpContext)!.Value;
            if (result.Enrolment!.Category!.Event!.OrganiserID != organiserId)
                return Forbid();

            result.FinishTime = dto.FinishTime;
            result.Position = dto.Position;
            result.Status = dto.Status;

            await _context.SaveChangesAsync();
            return Ok(result);
        }
    }
}
