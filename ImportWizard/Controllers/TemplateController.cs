using System.IO;
using System.Linq;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;            // your AppDbContext
using ImportWizard.Data.Models;
using ImportWizard.Data;     // SectionColumn entity

namespace ImportWizard.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TemplateController : ControllerBase
    {
        private readonly AppDbContext _ctx;

        public TemplateController(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        // GET /api/Template/download?columnIds=3,4,5
        [HttpGet("download")]
        public IActionResult Download([FromQuery] int[] columnIds)
        {
            // create workbook & worksheet
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Template");

            // write headers in row1
            for (int i = 0; i < columnIds.Length; i++)
            {
                var colEntity = _ctx.SectionColumns
                    .FirstOrDefault(sc => sc.ColumnId == columnIds[i]);
                var header = colEntity?.DisplayName ?? $"Column {columnIds[i]}";
                ws.Cell(1, i + 1).Value = header;
            }

            // stream out
            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            var content = ms.ToArray();
            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "template.xlsx"
            );
        }
    }
}
