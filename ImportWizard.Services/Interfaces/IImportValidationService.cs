// File: ImportWizard.Services.Interfaces/IImportValidationService.cs

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ImportWizard.Dtos.Validation;

namespace ImportWizard.Services.Interfaces
{
    /// <summary>
    /// Reads an uploaded Excel file, maps rows to ImportUserDto using the provided
    /// header→property mapping, runs validation, and returns per-row results.
    /// </summary>
    public interface IImportValidationService
    {
        /// <param name="file">The uploaded Excel file to validate.</param>
        /// <param name="headerToPropertyMap">
        /// A dictionary mapping Excel column headers to ImportUserDto property names.
        /// </param>
        Task<List<RowValidationResult>> ValidateFileAsync(
            IFormFile file,
            Dictionary<string, string> headerToPropertyMap
        );
    }
}
