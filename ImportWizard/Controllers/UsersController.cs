using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ImportWizard.Data;

namespace ImportWizard.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _db;
        public UsersController(AppDbContext db) => _db = db;

        // GET /api/users/count
        [HttpGet("count")]
        public async Task<ActionResult<int>> Count()
        {
            var total = await _db.Users.CountAsync();
            return Ok(total);
        }
    }
}
