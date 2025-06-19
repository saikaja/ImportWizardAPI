using System.Collections.Generic;
using System.Threading.Tasks;
using ImportWizard.Data.Models;

namespace ImportWizard.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(int id);
        Task AddAsync(Category entity);
        Task UpdateAsync(Category entity);
        Task RemoveAsync(Category entity);
    }
}