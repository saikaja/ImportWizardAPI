// File: ImportWizard.Services/Interfaces/IImportInputService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using ImportWizard.Dtos;

namespace ImportWizard.Services.Interfaces
{
    /// <summary>
    /// Handles mapping from raw user inputs (company name + columns)
    /// to ImportUserDto, and delegates to ImportResultService.
    /// </summary>
    public interface IImportInputService
    {
        Task<List<ImportResultDto>> ImportUsersAsync(List<ImportUserInputDto> inputs);
    }
}
