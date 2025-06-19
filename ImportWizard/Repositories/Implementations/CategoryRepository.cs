// Repositories/Implementations/CategoryRepository.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ImportWizard.Data;
using ImportWizard.Repositories.Interfaces;
using ImportWizard.Data.Models;

namespace ImportWizard.Repositories.Implementations
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _ctx;
        public CategoryRepository(AppDbContext ctx) => _ctx = ctx;

        public async Task<IEnumerable<Category>> GetAllAsync()
            => await _ctx.Categories.AsNoTracking().ToListAsync();

        public async Task<Category?> GetByIdAsync(int id)
            => await _ctx.Categories.FindAsync(id);

        public async Task AddAsync(Category entity)
            => await _ctx.Categories.AddAsync(entity);

        public Task UpdateAsync(Category entity)
        {
            _ctx.Categories.Update(entity);
            return Task.CompletedTask;
        }

        public Task RemoveAsync(Category entity)
        {
            _ctx.Categories.Remove(entity);
            return Task.CompletedTask;
        }
    }
}
