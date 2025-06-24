using ImportWizard.Data;
using ImportWizard.Data.Models;
using ImportWizard.Dtos;
using ImportWizard.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportWizard.Services.Implementations
{
    public class ImportResultService : IImportResultService
    {
        private readonly AppDbContext _db;
        public ImportResultService(AppDbContext db) => _db = db;

        public async Task<List<ImportResultDto>> ImportUsersAsync(List<ImportUserDto> dtos)
        {
            var emails = dtos.Select(d => d.Email.ToLower()).Distinct().ToList();
            var existing = await _db.Users
                .Where(u => emails.Contains(u.Email.ToLower()))
                .Select(u => u.Email.ToLower())
                .ToListAsync();
            var already = new HashSet<string>(existing);

            var compIds = dtos.Select(d => d.CompanyId).Distinct();
            var compNames = await _db.Companies
                .Where(c => compIds.Contains(c.CompanyId))
                .ToDictionaryAsync(c => c.CompanyId, c => c.Name);

            var results = new List<ImportResultDto>();
            var toInsert = new List<User>();

            foreach (var dto in dtos)
            {
                var key = dto.Email.ToLower();
                if (already.Contains(key))
                {
                    results.Add(new ImportResultDto
                    {
                        Email = dto.Email,
                        Inserted = false,
                        ErrorMessage = "Email already exists"
                    });
                    continue;
                }

                compNames.TryGetValue(dto.CompanyId, out var compName);

                toInsert.Add(new User
                {
                    Company = compName,
                    Location = dto.LocationCode,
                    EmployeeId = dto.EmployeeId,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Email = dto.Email,
                    Role = dto.Role,
                    Printer = dto.Printer,
                    ActivateNow = bool.TryParse(dto.Activate, out var a) && a,
                    Comments = dto.Comments
                });

                results.Add(new ImportResultDto
                {
                    Email = dto.Email,
                    Inserted = true
                });
            }

            if (toInsert.Any())
            {
                _db.Users.AddRange(toInsert);
                await _db.SaveChangesAsync();
            }

            return results;
        }
    }
}
