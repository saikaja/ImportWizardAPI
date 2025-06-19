// ImportWizard.Services.Implementations/CategoryHierarchyService.cs
using ImportWizard.Dtos;
using ImportWizard.Repositories.Interfaces;
using ImportWizard.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ImportWizard.Services.Implementations
{
    public class CategoryHierarchyService : ICategoryHierarchyService
    {
        private readonly ICategoryHierarchyRepository _repo;
        public CategoryHierarchyService(ICategoryHierarchyRepository repo)
            => _repo = repo;

        public Task<List<CategoryHierarchyDto>> GetAllAsync()
            => _repo.GetAllAsync();
    }
}
