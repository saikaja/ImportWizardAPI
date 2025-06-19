using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImportWizard.Data;
using ImportWizard.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ImportWizard.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompaniesController : ControllerBase
    {
        private readonly AppDbContext _db;

        public CompaniesController(AppDbContext db)
        {
            _db = db;
        }

        // GET: /api/companies
        [HttpGet]
        public async Task<ActionResult<List<CompanyDto>>> Get()
        {
            var list = await _db.Companies
                .AsNoTracking()
                .Select(c => new CompanyDto
                {
                    CompanyId = c.CompanyId,
                    Name = c.Name,
                    Description = c.Description,
                    IsActive = c.IsActive
                })
                .ToListAsync();

            return Ok(list);
        }
    }
}
