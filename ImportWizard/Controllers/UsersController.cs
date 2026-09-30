using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ImportWizard.Data;

namespace ImportWizard.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _db;
        public UsersController(AppDbContext db) => _db = db;

        // GET /api/users/count
        [HttpGet("count")]
        public async Task<ActionResult<int>> Count()
        {
            var total = await _db.Users.CountAsync();
            return Ok(total);
        }

        // POST /api/users/existing-emails   body: ["a@x.com", "b@x.com"]
        // Returns the given emails (trimmed, lower-cased) that already exist in imp.Users.
        // Uses the same duplicate rule as the queued import job, so the UI can tell
        // which rows that job will skip.
        [HttpPost("existing-emails")]
        public async Task<ActionResult<List<string>>> ExistingEmails([FromBody] List<string>? emails)
        {
            var keys = (emails ?? new List<string>())
                .Select(e => (e ?? string.Empty).Trim().ToLowerInvariant())
                .Where(e => e.Length > 0)
                .Distinct()
                .ToList();
            if (keys.Count == 0) return Ok(new List<string>());

            var existing = await _db.Users
                .Where(u => u.Email != null && keys.Contains(u.Email.Trim().ToLower()))
                .Select(u => u.Email.Trim().ToLower())
                .Distinct()
                .ToListAsync();
            return Ok(existing);
        }
    }
}
