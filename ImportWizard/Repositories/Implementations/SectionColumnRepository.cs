// Repositories/Implementations/SectionColumnRepository.cs
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ImportWizard.Data;
using ImportWizard.Repositories.Interfaces;
using ImportWizard.Data.Models;

namespace ImportWizard.Repositories.Implementations
{
    public class SectionColumnRepository : ISectionColumnRepository
    {
        private readonly AppDbContext _ctx;
        public SectionColumnRepository(AppDbContext ctx) => _ctx = ctx;

        public async Task<IEnumerable<SectionColumn>> GetAllAsync()
            => await _ctx.SectionColumns.AsNoTracking().ToListAsync();

        public async Task<SectionColumn?> GetByIdAsync(int id)
            => await _ctx.SectionColumns.FindAsync(id);

        public async Task<IEnumerable<SectionColumn>> GetBySectionIdAsync(int sectionId)
            => await _ctx.SectionColumns
                         .AsNoTracking()
                         .Where(sc => sc.SectionId == sectionId)
                         .ToListAsync();

        public async Task AddAsync(SectionColumn entity)
            => await _ctx.SectionColumns.AddAsync(entity);

        public Task UpdateAsync(SectionColumn entity)
        {
            _ctx.SectionColumns.Update(entity);
            return Task.CompletedTask;
        }

        public Task RemoveAsync(SectionColumn entity)
        {
            _ctx.SectionColumns.Remove(entity);
            return Task.CompletedTask;
        }
    }
}
