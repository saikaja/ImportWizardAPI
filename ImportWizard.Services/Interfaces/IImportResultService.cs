using System.Collections.Generic;
using System.Threading.Tasks;
using ImportWizard.Dtos;

namespace ImportWizard.Services.Interfaces
{
    public interface IImportResultService
    {
        Task<List<ImportResultDto>> ImportUsersAsync(List<ImportUserDto> dtos);
    }
}
