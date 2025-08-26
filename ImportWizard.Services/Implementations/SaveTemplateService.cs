using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using ImportWizard.Data;
using ImportWizard.Data.Models;
using ImportWizard.Dtos;
using ImportWizard.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportWizard.Services
{
    public class SaveTemplateService : ISaveTemplateService
    {
        private readonly AppDbContext _ctx;
        public SaveTemplateService(AppDbContext ctx) => _ctx = ctx;

        public async Task<SaveTemplateDto> SaveAsync(string name, string[] headers)
        {
            // 1) Build header-only workbook
            byte[] bytes;
            using (var wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add("Template");
                for (var i = 0; i < headers.Length; i++)
                    ws.Cell(1, i + 1).Value = headers[i];
                using var ms = new MemoryStream();
                wb.SaveAs(ms);
                bytes = ms.ToArray();
            }

            // 2) Upsert by name
            var tpl = await _ctx.Templates
                .FirstOrDefaultAsync(t => t.Name == name);
            if (tpl != null)
            {
                tpl.FileBytes = bytes;
                tpl.CreatedAt = DateTime.UtcNow;
                _ctx.Templates.Update(tpl);
            }
            else
            {
                tpl = new Template
                {
                    Name = name,
                    FileBytes = bytes,
                    CreatedAt = DateTime.UtcNow
                };
                await _ctx.Templates.AddAsync(tpl);
            }
            await _ctx.SaveChangesAsync();

            // 3) Return DTO
            return new SaveTemplateDto
            {
                TemplateId = tpl.TemplateId,
                Name = tpl.Name,
                CreatedAt = tpl.CreatedAt
            };
        }

        public async Task<IEnumerable<SaveTemplateDto>> GetAllAsync()
        {
            return await _ctx.Templates
                .OrderBy(t => t.Name)
                .Select(t => new SaveTemplateDto
                {
                    TemplateId = t.TemplateId,
                    Name = t.Name,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<(byte[] FileBytes, string FileName)> GetFileAsync(int templateId)
        {
            var tpl = await _ctx.Templates.FindAsync(templateId);
            if (tpl == null)
                throw new KeyNotFoundException($"Template {templateId} not found.");
            return (tpl.FileBytes, $"{tpl.Name}.xlsx");
        }
    }
}
