using System.Collections.Generic;
using System.Threading.Tasks;
using ImportWizard.Dtos;

namespace ImportWizard.Services.Interfaces
{
    public interface ISaveTemplateService
    {
        Task<SaveTemplateDto> SaveAsync(string name, string[] headers);
        Task<IEnumerable<SaveTemplateDto>> GetAllAsync();
        Task<(byte[] FileBytes, string FileName)> GetFileAsync(int templateId);
    }
}
