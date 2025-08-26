using System;
using System.Linq;
using System.Threading.Tasks;
using ImportWizard.Data;
using ImportWizard.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ImportWizard.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImportMasterController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ImportMasterController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("log")]
        public async Task<IActionResult> LogImport([FromBody] ImportMaster import)
        {
            import.SubmittedAt = DateTime.UtcNow;
            _context.ImportMasters.Add(import);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // GET /api/importmaster/paged?from=YYYY-MM-DD&to=YYYY-MM-DD&status=Success&pageNumber=1&pageSize=5
        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 5,
            [FromQuery] string? from = null,     // YYYY-MM-DD in America/Toronto
            [FromQuery] string? to = null,       // YYYY-MM-DD in America/Toronto
            [FromQuery] string? status = null    // "Success" | "Failure"
        )
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById("America/Toronto");
            DateTime? fromUtc = null;
            DateTime? toUtc = null;

            if (!string.IsNullOrWhiteSpace(from))
            {
                var fromLocal = DateTime.SpecifyKind(DateTime.Parse(from).Date, DateTimeKind.Unspecified);
                fromUtc = TimeZoneInfo.ConvertTimeToUtc(fromLocal, tz);
            }

            if (!string.IsNullOrWhiteSpace(to))
            {
                var toLocalEnd = DateTime.SpecifyKind(DateTime.Parse(to).Date.AddDays(1).AddTicks(-1), DateTimeKind.Unspecified);
                toUtc = TimeZoneInfo.ConvertTimeToUtc(toLocalEnd, tz);
            }

            var query = _context.ImportMasters.AsQueryable();

            if (fromUtc.HasValue)
                query = query.Where(i => i.SubmittedAt >= fromUtc.Value);

            if (toUtc.HasValue)
                query = query.Where(i => i.SubmittedAt <= toUtc.Value);

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(i => i.Status == status);

            var totalCount = await query.CountAsync();

            var imports = await query
                .OrderByDescending(i => i.SubmittedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                totalCount,
                pageSize,
                currentPage = pageNumber,
                imports
            });
        }
    }
}
