// CategorySectionController.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ImportWizard.Dtos;
using ImportWizard.Services.Interfaces;

namespace ImportWizard.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategorySectionController : ControllerBase
    {
        private readonly ICategorySectionService _svc;
        public CategorySectionController(ICategorySectionService svc) => _svc = svc;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategorySectionDto>>> GetAll() =>
            Ok(await _svc.GetAllAsync());

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CategorySectionDto>> Get(int id)
        {
            var dto = await _svc.GetByIdAsync(id);
            return dto == null ? NotFound() : Ok(dto);
        }

        [HttpGet("by-category/{categoryId:int}")]
        public async Task<ActionResult<IEnumerable<CategorySectionDto>>> ByCategory(int categoryId) =>
            Ok(await _svc.GetByCategoryIdAsync(categoryId));

        [HttpPost]
        public async Task<ActionResult<CategorySectionDto>> Create([FromBody] CategorySectionDto dto)
        {
            var created = await _svc.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = created.SectionId }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] CategorySectionDto dto)
        {
            if (id != dto.SectionId) return BadRequest();
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
