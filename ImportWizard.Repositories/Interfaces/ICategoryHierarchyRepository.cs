// ImportWizard.Repositories.Interfaces/ICategoryHierarchyRepository.cs
using ImportWizard.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ImportWizard.Repositories.Interfaces
{
    public interface ICategoryHierarchyRepository
    {
        /// <summary>
        /// Returns every Category, each with Sections and each Section with Columns.
        /// </summary>
        Task<List<CategoryHierarchyDto>> GetAllAsync();
    }
}
