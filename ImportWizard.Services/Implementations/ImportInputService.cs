// File: ImportWizard.Services/Implementations/ImportInputService.cs
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImportWizard.Data;
using ImportWizard.Data.Models;
using ImportWizard.Dtos;
using ImportWizard.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportWizard.Services.Implementations
{
    public class ImportInputService : IImportInputService
    {
        private readonly AppDbContext _db;
        private readonly IImportResultService _importSvc;

        public ImportInputService(AppDbContext db,
                                  IImportResultService importSvc)
        {
            _db = db;
            _importSvc = importSvc;
        }

        public async Task<List<ImportResultDto>> ImportUsersAsync(
            List<ImportUserInputDto> inputs)
        {
            var results = new List<ImportResultDto>();
            var toImport = new List<ImportUserDto>();

            foreach (var inp in inputs)
            {
                // Lookup company by name
                var company = await _db.Companies
                    .SingleOrDefaultAsync(c => c.Name == inp.Company);
                if (company == null)
                {
                    // Capture error for this row
                    results.Add(new ImportResultDto
                    {
                        Email = inp.Email,
                        Inserted = false,
                        ErrorMessage = $"Unknown company: '{inp.Company}'"
                    });
                    continue;
                }

                // Build the DTO for valid companies
                toImport.Add(new ImportUserDto
                {
                    CompanyId = company.CompanyId,
                    LocationCode = inp.LocationCode,
                    FirstName = inp.FirstName,
                    LastName = inp.LastName,
                    Email = inp.Email,
                    EmployeeId = inp.EmployeeId,
                    Role = inp.Role,
                    Printer = inp.Printer,
                    Activate = inp.Activate,
                    Comments = inp.Comments
                });
            }

            // Delegate all valid rows to your existing import logic
            if (toImport.Any())
            {
                var importResults = await _importSvc.ImportUsersAsync(toImport);
                results.AddRange(importResults);
            }

            return results;
        }
    }
}
