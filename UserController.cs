using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Helpers;

namespace RaceDay.Api.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly RaceDayContext _context;

        public UserController(RaceDayContext context)
        {
            _context = context;
        }

        public class UpdateProfileDto
        {
            public string FullName { get; set; }
            public string? PhoneNumber { get; set; }
        }

        // GET /api/users/me — anyone logged in, own profile
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            if (!SessionHelper.IsAuthenticated(HttpContext))
                return Unauthorized("You must be logged in.");

            var userId = SessionHelper.GetUserId(HttpContext)!.Value;

            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            return Ok(new
            {
                user.UserID,
                user.FullName,
                user.Email,
                user.Role,
                user.PhoneNumber,
                user.CreatedAt
            });
        }

        // PUT /api/users/me — anyone logged in, own profile
        [HttpPut("me")]
        public async Task<IActionResult> UpdateMyProfile(UpdateProfileDto dto)
        {
            if (!SessionHelper.IsAuthenticated(HttpContext))
                return Unauthorized("You must be logged in.");

            var userId = SessionHelper.GetUserId(HttpContext)!.Value;

            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            user.FullName = dto.FullName;
            user.PhoneNumber = dto.PhoneNumber;

            await _context.SaveChangesAsync();

            return Ok(new { user.UserID, user.FullName, user.Role, user.PhoneNumber });
        }

        // GET /api/users/{id} — Organiser only, view any user's public profile
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            if (!SessionHelper.IsAuthenticated(HttpContext))
                return Unauthorized("You must be logged in.");

            if (SessionHelper.GetRole(HttpContext) != "Organiser")
                return Forbid();

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            return Ok(new { user.UserID, user.FullName, user.Role });
        }
    }
}