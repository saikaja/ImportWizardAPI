// ImportWizard.WebApi/Controllers/ImportValidationController.cs

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ImportWizard.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace ImportWizard.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImportValidationController : ControllerBase
    {
        /// <summary>
        /// Represents the validation result for a single row in the import.
        /// </summary>
        public class RowValidationResult
        {
            public int Row { get; set; }
            public bool IsValid { get; set; }
            public string[] Errors { get; set; } = Array.Empty<string>();
        }

        /// <summary>
        /// Validate a batch of ImportUserDto rows against DataAnnotations and custom DB rules.
        /// </summary>
        /// <param name="rows">The list of rows to validate.</param>
        /// <returns>A list of RowValidationResult objects indicating success or failure for each row.</returns>
        [HttpPost("validateRows")]
        [ProducesResponseType(typeof(List<RowValidationResult>), 200)]
        [ProducesResponseType(400)]
        public IActionResult Validate([FromBody] List<ImportUserDto> rows)
        {
            if (rows == null || rows.Count == 0)
            {
                return BadRequest("No rows provided. Please POST a JSON array of ImportUserDto objects.");
            }

            var results = new List<RowValidationResult>();

            for (var i = 0; i < rows.Count; i++)
            {
                var row = rows[i];

                // Build a ValidationContext that can resolve AppDbContext for DTO-level validation
                var validationContext = new ValidationContext(
                    instance: row,
                    serviceProvider: HttpContext.RequestServices,
                    items: null
                );

                var validationResults = new List<ValidationResult>();

                // Runs both DataAnnotations and the IValidatableObject.Validate(...) implementation
                var isValid = Validator.TryValidateObject(
                    instance: row,
                    validationContext: validationContext,
                    validationResults: validationResults,
                    validateAllProperties: true
                );

                results.Add(new RowValidationResult
                {
                    Row = i,
                    IsValid = isValid,
                    Errors = validationResults
                                .Select(vr => vr.ErrorMessage!)
                                .ToArray()
                });
            }

            return Ok(results);
        }
    }
}
