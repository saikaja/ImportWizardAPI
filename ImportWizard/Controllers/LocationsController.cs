// File: ImportWizard.Api/Controllers/LocationsController.cs
using Microsoft.AspNetCore.Mvc;
using ImportWizard.Data;               // your DbContext namespace
using ImportWizard.Data.Models;        // for Location entity
using System.Linq;

namespace ImportWizard.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocationsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public LocationsController(AppDbContext db) => _db = db;

        // GET /api/locations
        [HttpGet]
        public IActionResult GetAll()
        {
            var list = _db.Locations
                .Select(l => new {
                    companyId = l.CompanyId,
                    locationCode = l.LocationCode.Trim()
                })
                .ToList();
            return Ok(list);
        }
    }
}
