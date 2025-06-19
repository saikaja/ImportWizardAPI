// Repositories/Implementations/CategorySectionRepository.cs
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ImportWizard.Data;
using ImportWizard.Repositories.Interfaces;
using ImportWizard.Data.Models;

namespace ImportWizard.Repositories.Implementations
{
    public class CategorySectionRepository : ICategorySectionRepository
    {
        private readonly AppDbContext _ctx;
        public CategorySectionRepository(AppDbContext ctx) => _ctx = ctx;

        public async Task<IEnumerable<CategorySection>> GetAllAsync()
            => await _ctx.CategorySections.AsNoTracking().ToListAsync();

        public async Task<CategorySection?> GetByIdAsync(int id)
            => await _ctx.CategorySections.FindAsync(id);

        public async Task<IEnumerable<CategorySection>> GetByCategoryIdAsync(int categoryId)
            => await _ctx.CategorySections
                         .AsNoTracking()
                         .Where(cs => cs.CategoryId == categoryId)
                         .ToListAsync();

        public async Task AddAsync(CategorySection entity)
            => await _ctx.CategorySections.AddAsync(entity);

        public Task UpdateAsync(CategorySection entity)
        {
            _ctx.CategorySections.Update(entity);
            return Task.CompletedTask;
        }

        public Task RemoveAsync(CategorySection entity)
        {
            _ctx.CategorySections.Remove(entity);
            return Task.CompletedTask;
        }
    }
}
