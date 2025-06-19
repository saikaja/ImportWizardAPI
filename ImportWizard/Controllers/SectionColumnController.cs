// SectionColumnController.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ImportWizard.Dtos;
using ImportWizard.Services.Interfaces;

namespace ImportWizard.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SectionColumnController : ControllerBase
    {
        private readonly ISectionColumnService _svc;
        public SectionColumnController(ISectionColumnService svc) => _svc = svc;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SectionColumnDto>>> GetAll() =>
            Ok(await _svc.GetAllAsync());

        [HttpGet("{id:int}")]
        public async Task<ActionResult<SectionColumnDto>> Get(int id)
        {
            var dto = await _svc.GetByIdAsync(id);
            return dto == null ? NotFound() : Ok(dto);
        }

        [HttpGet("by-section/{sectionId:int}")]
        public async Task<ActionResult<IEnumerable<SectionColumnDto>>> BySection(int sectionId) =>
            Ok(await _svc.GetBySectionIdAsync(sectionId));

        [HttpPost]
        public async Task<ActionResult<SectionColumnDto>> Create([FromBody] SectionColumnDto dto)
        {
            var created = await _svc.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = created.ColumnId }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] SectionColumnDto dto)
        {
            if (id != dto.ColumnId) return BadRequest();
            await _svc.UpdateAsync(dto);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _svc.DeleteAsync(id);
            return NoContent();
        }
    }
}
