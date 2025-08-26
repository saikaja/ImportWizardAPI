using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImportWizard.Data;
using ImportWizard.Data.Models;
using ImportWizard.Dtos;
using ImportWizard.Services.Interfaces;
using ImportWizard.WebApi.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ImportWizard.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImportResultController : ControllerBase
    {
        private readonly IImportResultService _importSvc;   // existing synchronous path
        private readonly IMessagePublisher _publisher;      // Service Bus publisher
        private readonly AppDbContext _db;                  // for ImportMaster row

        public ImportResultController(
            IImportResultService importSvc,
            IMessagePublisher publisher,
            AppDbContext db)
        {
            _importSvc = importSvc;
            _publisher = publisher;
            _db = db;
        }

        // NEW: enqueue-only (one message per selected row)
        // POST /api/importresult/enqueue-users?fileName=Full%20User.xlsx
        // Returns 202 Accepted { queued, importMasterId }
        [HttpPost("enqueue-users")]
        [ProducesResponseType(typeof(object), StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> EnqueueUsers(
            [FromBody] List<ImportUserInputDto> inputs,
            [FromQuery] string? fileName = null)
        {
            if (inputs == null || inputs.Count == 0)
                return BadRequest("No data provided.");

            // 1) Create ImportMaster row as Queued
            var master = new ImportMaster
            {
                FileName = string.IsNullOrWhiteSpace(fileName) ? "unknown" : fileName,
                SubmittedAt = System.DateTime.UtcNow,
                Status = "Queued"
            };
            _db.ImportMasters.Add(master);
            await _db.SaveChangesAsync();

            // 2) Enqueue ONE message PER ROW; mark the final message with isLast=true
            for (int i = 0; i < inputs.Count; i++)
            {
                var dto = inputs[i];
                var envelope = new
                {
                    importMasterId = master.ImportId,
                    isLast = (i == inputs.Count - 1),
                    payload = dto
                };
                await _publisher.PublishAsync(envelope);
            }

            // 3) Tell client it's async work
            return Accepted(new { queued = inputs.Count, importMasterId = master.ImportId });
        }

        // EXISTING: synchronous import (kept for backward compatibility)
        // POST /api/importresult/users
        [HttpPost("users")]
        [ProducesResponseType(typeof(List<ImportResultDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ImportUsers([FromBody] List<ImportUserInputDto> inputs)
        {
            if (inputs == null || !inputs.Any())
                return BadRequest("No data provided.");

            var dtos = inputs.Select(i => new ImportUserDto
            {
                CompanyId = int.TryParse(i.Company, out var cid) ? cid : 0,
                CompanyName = i.Company?.Trim() ?? string.Empty,
                LocationCode = i.LocationCode,
                FirstName = i.FirstName,
                LastName = i.LastName,
                Email = i.Email,
                EmployeeId = i.EmployeeId,
                Role = i.Role,
                Printer = i.Printer,
                Activate = i.Activate,
                Comments = i.Comments
            }).ToList();

            var results = await _importSvc.ImportUsersAsync(dtos);
            return Ok(results);
        }

        // NEW: minimal status endpoint (no schema changes)
        // GET /api/importresult/status/{id}
        [HttpGet("status/{id:int}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStatus([FromRoute] int id)
        {
            var master = await _db.ImportMasters.FindAsync(id);
            if (master is null) return NotFound();

            return Ok(new
            {
                importMasterId = master.ImportId,
                status = master.Status,
                fileName = master.FileName,
                submittedAt = master.SubmittedAt
            });
        }
    }
}
