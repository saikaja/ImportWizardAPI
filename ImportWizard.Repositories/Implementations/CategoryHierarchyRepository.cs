// ImportWizard.Repositories.Implementations/CategoryHierarchyRepository.cs
using ImportWizard.Data;
using ImportWizard.Dtos;
using ImportWizard.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ImportWizard.Repositories.Implementations
{
    public class CategoryHierarchyRepository : ICategoryHierarchyRepository
    {
        private readonly AppDbContext _ctx;

        public CategoryHierarchyRepository(AppDbContext ctx)
            => _ctx = ctx;

        public async Task<List<CategoryHierarchyDto>> GetAllAsync()
        {
            // load each table flat
            var cats = await _ctx.Categories.ToListAsync();
            var secs = await _ctx.CategorySections.ToListAsync();
            var columns = await _ctx.SectionColumns.ToListAsync();

            // project into DTO tree
            return cats.Select(cat => new CategoryHierarchyDto
            {
                CategoryId = cat.CategoryId,
                Name = cat.Name,
                Description = cat.Description,
                Sections = secs
                    .Where(s => s.CategoryId == cat.CategoryId)
                    .Select(s => new SectionHierarchyDto
                    {
                        SectionId = s.SectionId,
                        CategoryId = s.CategoryId,
                        SectionName = s.SectionName,
                        SectionDescription = s.SectionDescription,
                        IsActive = s.IsActive,
                        Columns = columns
                            .Where(c => c.SectionId == s.SectionId)
                            .Select(c => new SectionColumnDto
                            {
                                ColumnId = c.ColumnId,
                                SectionId = c.SectionId,
                                ColumnName = c.ColumnName,
                                DisplayName = c.DisplayName,
                                DataType = c.DataType,
                                DbColumnName = c.DbColumnName,
                                IsIdentifier = c.IsIdentifier,
                                Options = string.IsNullOrWhiteSpace(c.Options)
                                     ? null
                                     : JsonSerializer.Deserialize<OptionsDto>(c.Options)
                            })
                            .ToList()
                    })
                    .ToList()
            })
            .ToList();
        }
    }
}
