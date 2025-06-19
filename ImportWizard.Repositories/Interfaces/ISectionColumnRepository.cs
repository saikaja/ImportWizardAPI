using System.Collections.Generic;
using System.Threading.Tasks;
using ImportWizard.Data.Models;

namespace ImportWizard.Repositories.Interfaces
{
    public interface ISectionColumnRepository
    {
        Task<IEnumerable<SectionColumn>> GetAllAsync();
        Task<SectionColumn?> GetByIdAsync(int id);
        Task<IEnumerable<SectionColumn>> GetBySectionIdAsync(int sectionId);
        Task AddAsync(SectionColumn entity);
        Task UpdateAsync(SectionColumn entity);
        Task RemoveAsync(SectionColumn entity);
    }
}
