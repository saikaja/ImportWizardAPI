// ImportWizard.Services.Implementations/SectionColumnService.cs
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ImportWizard.Data;                            // for AppDbContext
using ImportWizard.Data.Models;                     // for SectionColumn entity
using ImportWizard.Dtos;                            // for SectionColumnDto & OptionsDto
using ImportWizard.Repositories.Interfaces;         // for ISectionColumnRepository
using ImportWizard.Services.Interfaces;             // for ISectionColumnService

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
            return ents.Select(MapToDto);
        }

        public async Task<SectionColumnDto?> GetByIdAsync(int id)
        {
            var e = await _repo.GetByIdAsync(id);
            return e == null ? null : MapToDto(e);
        }

        public async Task<IEnumerable<SectionColumnDto>> GetBySectionIdAsync(int sectionId)
        {
            var ents = await _repo.GetBySectionIdAsync(sectionId);
            return ents.Select(MapToDto);
        }

        public async Task<SectionColumnDto> CreateAsync(SectionColumnDto dto)
        {
            var e = new SectionColumn
            {
                SectionId = dto.SectionId,
                ColumnName = dto.ColumnName,
                DisplayName = dto.DisplayName,
                DataType = dto.DataType,
                DbColumnName = dto.DbColumnName,
                IsIdentifier = dto.IsIdentifier,
                // serialize OptionsDto → JSON string
                Options = dto.Options is null
                                ? null
                                : JsonSerializer.Serialize(dto.Options)
            };

            await _repo.AddAsync(e);
            await _ctx.SaveChangesAsync();

            // push the new PK back
            dto.ColumnId = e.ColumnId;
            return dto;
        }

        public async Task UpdateAsync(SectionColumnDto dto)
        {
            var e = new SectionColumn
            {
                ColumnId = dto.ColumnId,   // ensure EF knows which row to update
                SectionId = dto.SectionId,
                ColumnName = dto.ColumnName,
                DisplayName = dto.DisplayName,
                DataType = dto.DataType,
                DbColumnName = dto.DbColumnName,
                IsIdentifier = dto.IsIdentifier,
                // serialize OptionsDto → JSON string
                Options = dto.Options is null
                                ? null
                                : JsonSerializer.Serialize(dto.Options)
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

        /// <summary>
        /// Helper to map EF entity → DTO, including JSON deserialization
        /// </summary>
        private static SectionColumnDto MapToDto(SectionColumn e)
        {
            return new SectionColumnDto
            {
                ColumnId = e.ColumnId,
                SectionId = e.SectionId,
                ColumnName = e.ColumnName,
                DisplayName = e.DisplayName,
                DataType = e.DataType,
                DbColumnName = e.DbColumnName,
                IsIdentifier = e.IsIdentifier,
                // deserialize JSON string → OptionsDto
                Options = string.IsNullOrWhiteSpace(e.Options)
                                ? null
                                : JsonSerializer.Deserialize<OptionsDto>(e.Options)
            };
        }
    }
}
