// CategorySectionService.cs
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImportWizard.Data.Models;
using ImportWizard.Dtos;
using ImportWizard.Repositories.Interfaces;
using ImportWizard.Services.Interfaces;

namespace ImportWizard.Services.Implementations
{
    public class CategorySectionService : ICategorySectionService
    {
        private readonly ICategorySectionRepository _repo;
        private readonly AppDbContext _ctx;

        public CategorySectionService(ICategorySectionRepository repo, AppDbContext ctx)
        {
            _repo = repo;
            _ctx = ctx;
        }

        public async Task<IEnumerable<CategorySectionDto>> GetAllAsync()
        {
            var ents = await _repo.GetAllAsync();
            return ents.Select(e => new CategorySectionDto
            {
                SectionId = e.SectionId,
                CategoryId = e.CategoryId,
                SectionName = e.SectionName,
                SectionDescription = e.SectionDescription,
                IsActive = e.IsActive
            });
        }

        public async Task<CategorySectionDto?> GetByIdAsync(int id)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) return null;
            return new CategorySectionDto
            {
                SectionId = e.SectionId,
                CategoryId = e.CategoryId,
                SectionName = e.SectionName,
                SectionDescription = e.SectionDescription,
                IsActive = e.IsActive
            };
        }

        public async Task<IEnumerable<CategorySectionDto>> GetByCategoryIdAsync(int categoryId)
        {
            var ents = await _repo.GetByCategoryIdAsync(categoryId);
            return ents.Select(e => new CategorySectionDto
            {
                SectionId = e.SectionId,
                CategoryId = e.CategoryId,
                SectionName = e.SectionName,
                SectionDescription = e.SectionDescription,
                IsActive = e.IsActive
            });
        }

        public async Task<CategorySectionDto> CreateAsync(CategorySectionDto dto)
        {
            var e = new CategorySection
            {
                CategoryId = dto.CategoryId,
                SectionName = dto.SectionName,
                SectionDescription = dto.SectionDescription,
                IsActive = dto.IsActive
            };
            await _repo.AddAsync(e);
            await _ctx.SaveChangesAsync();
            dto.SectionId = e.SectionId;
            return dto;
        }

        public async Task UpdateAsync(CategorySectionDto dto)
        {
            var e = new CategorySection
            {
                SectionId = dto.SectionId,
                CategoryId = dto.CategoryId,
                SectionName = dto.SectionName,
                SectionDescription = dto.SectionDescription,
                IsActive = dto.IsActive
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
