using ImportWizard.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ImportWizard.Services.Interfaces
{
    public interface ICategoryHierarchyService
    {

        Task<List<CategoryHierarchyDto>> GetAllAsync();
    }
}
