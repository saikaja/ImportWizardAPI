// File: ImportWizard.WebApi/Controllers/ImportValidationController.cs

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Threading.Tasks;
using ImportWizard.Services.Interfaces;       // IImportValidationService
using ImportWizard.Dtos.Validation;           // RowValidationResult
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ImportWizard.WebApi.Controllers
{
    /// <summary>
    /// Binds the multipart/form-data payload: Excel file + mappings JSON.
    /// </summary>
    public class ValidateRowsRequest
    {
        [Required]
        [FromForm(Name = "file")]
        public IFormFile File { get; set; } = default!;

        [Required]
        [FromForm(Name = "mappings")]
        public string Mappings { get; set; } = default!;

        public Dictionary<string, string> GetMappingDictionary()
        {
            if (string.IsNullOrWhiteSpace(Mappings))
                throw new InvalidOperationException("Mappings JSON is empty.");

            return JsonSerializer
                .Deserialize<Dictionary<string, string>>(Mappings)
                ?? throw new InvalidOperationException("Unable to parse mappings JSON.");
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class ImportValidationController : ControllerBase
    {
        private readonly IImportValidationService _validationService;

        public ImportValidationController(IImportValidationService validationService)
        {
            _validationService = validationService;
        }

        [HttpPost("validateRows")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(List<RowValidationResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ValidateRows([FromForm] ValidateRowsRequest req)
        {
            // 1) Ensure we got a file
            if (req.File == null || req.File.Length == 0)
                return BadRequest("No file uploaded.");

            // 2) Parse mappings JSON
            Dictionary<string, string> mappingDict;
            try
            {
                mappingDict = req.GetMappingDictionary();
            }
            catch (Exception ex)
            {
                return BadRequest($"Unable to parse mappings JSON: {ex.Message}");
            }

            // 3) Delegate to your service (reads Excel, maps to DTOs, runs DataAnnotations)
            var results = await _validationService.ValidateFileAsync(req.File, mappingDict);

            // 4) Return the list of RowValidationResult
            return Ok(results);
        }
    }
}
