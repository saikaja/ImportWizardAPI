// File: ImportWizard.WebApi/Controllers/ImportResultController.cs

using System.Collections.Generic;
using System.Threading.Tasks;
using ImportWizard.Dtos;
using ImportWizard.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ImportWizard.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImportResultController : ControllerBase
    {
        private readonly IImportInputService _inputSvc;

        public ImportResultController(IImportInputService inputSvc)
            => _inputSvc = inputSvc;

        [HttpPost("users")]
        [ProducesResponseType(typeof(List<ImportResultDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ImportUsers(
            [FromBody] List<ImportUserInputDto> inputs)
        {
            if (inputs == null || inputs.Count == 0)
                return BadRequest("No data provided.");

            var results = await _inputSvc.ImportUsersAsync(inputs);
            return Ok(results);
        }
    }
}
