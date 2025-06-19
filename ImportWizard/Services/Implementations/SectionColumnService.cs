// SectionColumnService.cs
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImportWizard.Data.Models;
using ImportWizard.Dtos;
using ImportWizard.Repositories.Interfaces;
using ImportWizard.Services.Interfaces;

namespace ImportWizard.Services.Implementations
{
    public class SectionColumnService : ISectionColumnService
    {
        private readonly ISectionColumnRepository _repo;
        private readonly AppDbContext _ctx;

        public SectionColumnService(ISectionColumnRepository repo, AppDbContext ctx)
        {
            _repo = repo;
            _ctx = ctx;
        }

        public async Task<IEnumerable<SectionColumnDto>> GetAllAsync()
        {
            var ents = await _repo.GetAllAsync();
            return ents.Select(e => new SectionColumnDto
            {
                ColumnId = e.ColumnId,
                SectionId = e.SectionId,
                ColumnName = e.ColumnName,
                DisplayName = e.DisplayName,
                IsRequired = e.IsRequired,
                DataType = e.DataType,
                Format = e.Format,
                Options = e.Options
            });
        }

        public async Task<SectionColumnDto?> GetByIdAsync(int id)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) return null;
            return new SectionColumnDto
            {
                ColumnId = e.ColumnId,
                SectionId = e.SectionId,
                ColumnName = e.ColumnName,
                DisplayName = e.DisplayName,
                IsRequired = e.IsRequired,
                DataType = e.DataType,
                Format = e.Format,
                Options = e.Options
            };
        }

        public async Task<IEnumerable<SectionColumnDto>> GetBySectionIdAsync(int sectionId)
        {
            var ents = await _repo.GetBySectionIdAsync(sectionId);
            return ents.Select(e => new SectionColumnDto
            {
                ColumnId = e.ColumnId,
                SectionId = e.SectionId,
                ColumnName = e.ColumnName,
                DisplayName = e.DisplayName,
                IsRequired = e.IsRequired,
                DataType = e.DataType,
                Format = e.Format,
                Options = e.Options
            });
        }

        public async Task<SectionColumnDto> CreateAsync(SectionColumnDto dto)
        {
            var e = new SectionColumn
            {
                SectionId = dto.SectionId,
                ColumnName = dto.ColumnName,
                DisplayName = dto.DisplayName,
                IsRequired = dto.IsRequired,
                DataType = dto.DataType,
                Format = dto.Format,
                Options = dto.Options
            };
            await _repo.AddAsync(e);
            await _ctx.SaveChangesAsync();
            dto.ColumnId = e.ColumnId;
            return dto;
        }

        public async Task UpdateAsync(SectionColumnDto dto)
        {
            var e = new SectionColumn
            {
                ColumnId = dto.ColumnId,
                SectionId = dto.SectionId,
                ColumnName = dto.ColumnName,
                DisplayName = dto.DisplayName,
                IsRequired = dto.IsRequired,
                DataType = dto.DataType,
                Format = dto.Format,
                Options = dto.Options
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
    