// ICategorySectionService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using ImportWizard.Dtos;

namespace ImportWizard.Services.Interfaces
{
    public interface ICategorySectionService
    {
        Task<IEnumerable<CategorySectionDto>> GetAllAsync();
        Task<CategorySectionDto?> GetByIdAsync(int id);
        Task<IEnumerable<CategorySectionDto>> GetByCategoryIdAsync(int categoryId);
        Task<CategorySectionDto> CreateAsync(CategorySectionDto dto);
        Task UpdateAsync(CategorySectionDto dto);
        Task DeleteAsync(int id);
    }
}
