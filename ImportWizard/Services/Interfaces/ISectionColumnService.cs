// ISectionColumnService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using ImportWizard.Dtos;

namespace ImportWizard.Services.Interfaces
{
    public interface ISectionColumnService
    {
        Task<IEnumerable<SectionColumnDto>> GetAllAsync();
        Task<SectionColumnDto?> GetByIdAsync(int id);
        Task<IEnumerable<SectionColumnDto>> GetBySectionIdAsync(int sectionId);
        Task<SectionColumnDto> CreateAsync(SectionColumnDto dto);
        Task UpdateAsync(SectionColumnDto dto);
        Task DeleteAsync(int id);
    }
}
