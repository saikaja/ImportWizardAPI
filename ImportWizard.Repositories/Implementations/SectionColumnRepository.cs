// ImportWizard.Repositories.Implementations/SectionColumnRepository.cs
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImportWizard.Data;
using ImportWizard.Data.Models;
using ImportWizard.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportWizard.Repositories.Implementations
{
    public class SectionColumnRepository : ISectionColumnRepository
    {
        private readonly AppDbContext _ctx;
        public SectionColumnRepository(AppDbContext ctx) => _ctx = ctx;

        public async Task<IEnumerable<SectionColumn>> GetAllAsync()
        {
            return await _ctx.SectionColumns.ToListAsync();
        }

        public async Task<SectionColumn?> GetByIdAsync(int id)
        {
            return await _ctx.SectionColumns
                             .FirstOrDefaultAsync(sc => sc.ColumnId == id);
        }

        public async Task<IEnumerable<SectionColumn>> GetBySectionIdAsync(int sectionId)
        {
            return await _ctx.SectionColumns
                             .Where(sc => sc.SectionId == sectionId)
                             .ToListAsync();
        }

        public async Task AddAsync(SectionColumn entity)
        {
            await _ctx.SectionColumns.AddAsync(entity);
        }

        public async Task UpdateAsync(SectionColumn entity)
        {
            _ctx.SectionColumns.Update(entity);
        }

        public async Task RemoveAsync(SectionColumn entity)
        {
            _ctx.SectionColumns.Remove(entity);
        }
    }
}
