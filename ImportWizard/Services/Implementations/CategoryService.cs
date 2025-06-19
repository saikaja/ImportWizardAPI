// CategoryService.cs
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImportWizard.Data.Models;
using ImportWizard.Dtos;
using ImportWizard.Repositories.Interfaces;
using ImportWizard.Services.Interfaces;

namespace ImportWizard.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repo;
        private readonly AppDbContext _ctx;

        public CategoryService(ICategoryRepository repo, AppDbContext ctx)
        {
            _repo = repo;
            _ctx = ctx;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            var ents = await _repo.GetAllAsync();
            return ents.Select(e => new CategoryDto
            {
                CategoryId = e.CategoryId,
                Name = e.Name,
                Description = e.Description
            });
        }

        public async Task<CategoryDto?> GetByIdAsync(int id)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) return null;
            return new CategoryDto
            {
                CategoryId = e.CategoryId,
                Name = e.Name,
                Description = e.Description
            };
        }

        public async Task<CategoryDto> CreateAsync(CategoryDto dto)
        {
            var e = new Category
            {
                Name = dto.Name,
                Description = dto.Description
            };
            await _repo.AddAsync(e);
            await _ctx.SaveChangesAsync();
            dto.CategoryId = e.CategoryId;
            return dto;
        }

        public async Task UpdateAsync(CategoryDto dto)
        {
            var e = new Category
            {
                CategoryId = dto.CategoryId,
                Name = dto.Name,
                Description = dto.Description
            };
            await _repo.UpdateAsync(e);
            await _ctx.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) return;
            await _repo.RemoveAsync(e);
            await _ctx.SaveChangesAsync();
        }
    }
}
