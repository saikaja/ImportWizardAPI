// File: ImportWizard.Services.Implementations/ImportResultService.cs

using ImportWizard.Data;
using ImportWizard.Data.Models;
using ImportWizard.Dtos;
using ImportWizard.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
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
            // Load all companies upfront
            var companies = await _db.Companies.ToListAsync();
            var idToName = companies.ToDictionary(c => c.CompanyId, c => c.Name);
            var nameLookup = companies
              .GroupBy(c => c.Name, StringComparer.OrdinalIgnoreCase)
              .ToDictionary(g => g.Key, g => g.First().Name, StringComparer.OrdinalIgnoreCase);

            // Deduplicate existing emails
            var emails = dtos.Select(d => d.Email.ToLower()).Distinct().ToList();
            var existing = await _db.Users
                .Where(u => emails.Contains(u.Email.ToLower()))
                .Select(u => u.Email.ToLower())
                .ToListAsync();
            var already = new HashSet<string>(existing);

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

                // Resolve company by ID or by Name fallback
                string compName = null;
                if (dto.CompanyId > 0 && idToName.TryGetValue(dto.CompanyId, out var byId))
                    compName = byId;
                else if (!string.IsNullOrWhiteSpace(dto.CompanyName)
                      && nameLookup.TryGetValue(dto.CompanyName.Trim(), out var byName))
                    compName = byName;

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
