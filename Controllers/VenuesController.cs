using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;

namespace RaceDay.Api.Controllers
{
    [ApiController]
    [Route("api/venues")]
    public class VenuesController : ControllerBase
    {
        private readonly RaceDayContext _context;

        public VenuesController(RaceDayContext context)
        {
            _context = context;
        }

        // GET /api/venues - lists venues for the event form
        [HttpGet]
        public async Task<IActionResult> GetVenues()
        {
            var venues = await _context.Venues
                .Select(v => new { v.VenueID, v.VenueName, v.City, v.Province })
                .ToListAsync();

            return Ok(venues);
        }
    }
}
