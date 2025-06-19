using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ImportWizard.Dtos;
using ImportWizard.Services.Interfaces;

namespace ImportWizard.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoryHierarchyController : ControllerBase
    {
        private readonly ICategoryHierarchyService _hierSvc;
        public CategoryHierarchyController(ICategoryHierarchyService hierSvc)
            => _hierSvc = hierSvc;

        /// <summary>
        /// GET /api/CategoryHierarchy
        /// Returns all categories with their sections and columns.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<CategoryHierarchyDto>>> Get()
        {
            var tree = await _hierSvc.GetAllAsync();
            return Ok(tree);
        }
    }
}
