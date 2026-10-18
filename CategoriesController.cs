using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using RaceDay.Api.Helpers;

namespace RaceDay.Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class CategoriesController : ControllerBase
    {
        private readonly RaceDayContext _context;

        public CategoriesController(RaceDayContext context)
        {
            _context = context;
        }

        public class CategoryDto
        {
            public string CategoryName { get; set; }
            public decimal DistanceKM { get; set; }
            public decimal EntryFee { get; set; }
        }

        // GET /api/events/{eventId}/categories — anyone can view
        [HttpGet("events/{eventId}/categories")]
        public async Task<IActionResult> GetCategories(int eventId)
        {
            var categories = await _context.Categories
                .Where(c => c.EventID == eventId)
                .ToListAsync();

            return Ok(categories);
        }

        // POST /api/events/{eventId}/categories — Organiser only, must own the event
        [HttpPost("events/{eventId}/categories")]
        public async Task<IActionResult> CreateCategory(int eventId, CategoryDto dto)
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

            var category = new Category
            {
                EventID = eventId,
                CategoryName = dto.CategoryName,
                DistanceKM = dto.DistanceKM,
                EntryFee = dto.EntryFee
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return StatusCode(201, category);
        }

        // PUT /api/categories/{id} — Organiser only, must own the parent event
        [HttpPut("categories/{id}")]
        public async Task<IActionResult> UpdateCategory(int id, CategoryDto dto)
        {
            if (!SessionHelper.IsAuthenticated(HttpContext))
                return Unauthorized("You must be logged in.");

            if (SessionHelper.GetRole(HttpContext) != "Organiser")
                return Forbid();

            var category = await _context.Categories
                .Include(c => c.Event)
                .FirstOrDefaultAsync(c => c.CategoryID == id);

            if (category == null) return NotFound();

            var organiserId = SessionHelper.GetUserId(HttpContext)!.Value;
            if (category.Event!.OrganiserID != organiserId)
                return Forbid();

            category.CategoryName = dto.CategoryName;
            category.DistanceKM = dto.DistanceKM;
            category.EntryFee = dto.EntryFee;

            await _context.SaveChangesAsync();
            return Ok(category);
        }

        // DELETE /api/categories/{id} — Organiser only, must own the parent event
        [HttpDelete("categories/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            if (!SessionHelper.IsAuthenticated(HttpContext))
                return Unauthorized("You must be logged in.");

            if (SessionHelper.GetRole(HttpContext) != "Organiser")
                return Forbid();

            var category = await _context.Categories
                .Include(c => c.Event)
                .FirstOrDefaultAsync(c => c.CategoryID == id);

            if (category == null) return NotFound();

            var organiserId = SessionHelper.GetUserId(HttpContext)!.Value;
            if (category.Event!.OrganiserID != organiserId)
                return Forbid();

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}