using System.Collections.Generic;
using System.Threading.Tasks;
using ImportWizard.Data.Models;

namespace ImportWizard.Repositories.Interfaces
{
    public interface ICategorySectionRepository
    {
        Task<IEnumerable<CategorySection>> GetAllAsync();
        Task<CategorySection?> GetByIdAsync(int id);
        Task<IEnumerable<CategorySection>> GetByCategoryIdAsync(int categoryId);
        Task AddAsync(CategorySection entity);
        Task UpdateAsync(CategorySection entity);
        Task RemoveAsync(CategorySection entity);
    }
}