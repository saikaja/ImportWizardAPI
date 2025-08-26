using System.Collections.Generic;
using System.Threading.Tasks;
using ImportWizard.Dtos;
using ImportWizard.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ImportWizard.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SaveTemplateController : ControllerBase
    {
        private readonly ISaveTemplateService _svc;
        public SaveTemplateController(ISaveTemplateService svc) => _svc = svc;

        public class SaveTemplateRequest
        {
            public string Name { get; set; }
            public string[] Headers { get; set; }
        }

        [HttpPost]
        public async Task<ActionResult<SaveTemplateDto>> Post([FromBody] SaveTemplateRequest req)
            => Ok(await _svc.SaveAsync(req.Name, req.Headers));

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SaveTemplateDto>>> GetAll()
            => Ok(await _svc.GetAllAsync());

        [HttpGet("{id}/download")]
        public async Task<IActionResult> Download(int id)
        {
            var (bytes, fileName) = await _svc.GetFileAsync(id);
            return File(
                bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName
            );
        }
    }
}
