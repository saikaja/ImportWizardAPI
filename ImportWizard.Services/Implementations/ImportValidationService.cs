// File: ImportWizard.Services.Implementations/ImportValidationService.cs
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ExcelDataReader;
using ImportWizard.Data;
using ImportWizard.Dtos;
using ImportWizard.Dtos.Validation;      // for RowValidationResult
using ImportWizard.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace ImportWizard.Services.Implementations
{
    public class ImportValidationService : IImportValidationService
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger<ImportValidationService> _logger;

        public ImportValidationService(
            IServiceProvider provider,
            ILogger<ImportValidationService> logger)
        {
            _provider = provider;
            _logger = logger;
            // required for ExcelDataReader (e.g. .xlsx support)
            System.Text.Encoding.RegisterProvider(
                System.Text.CodePagesEncodingProvider.Instance);
        }

        public async Task<List<RowValidationResult>> ValidateFileAsync(
            IFormFile file,
            Dictionary<string, string> headerToPropertyMap
        )
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("No Excel file uploaded.");

            var db = _provider.GetService<AppDbContext>()
                     ?? throw new InvalidOperationException("AppDbContext not available");

            // build CompanyName → CompanyId lookup
            var companies = await Task.Run(() =>
                db.Companies
                  .Select(c => new { c.CompanyId, Name = c.Name.Trim() })
                  .ToList());
            var companyLookup = companies
                .ToDictionary(x => x.Name, x => x.CompanyId,
                              StringComparer.OrdinalIgnoreCase);

            // read Excel into a DataTable
            DataTable table;
            using (var stream = file.OpenReadStream())
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                table = reader
                  .AsDataSet(new ExcelDataSetConfiguration
                  {
                      ConfigureDataTable = _ => new ExcelDataTableConfiguration
                      {
                          UseHeaderRow = true
                      }
                  })
                  .Tables[0];
            }

            // reflect all public props on ImportUserDto
            var dtoType = typeof(ImportUserDto);
            var props = dtoType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .ToDictionary(p => p.Name, p => p, StringComparer.OrdinalIgnoreCase);

            var results = new List<RowValidationResult>();

            for (int rowIndex = 0; rowIndex < table.Rows.Count; rowIndex++)
            {
                var dr = table.Rows[rowIndex];
                var dto = Activator.CreateInstance<ImportUserDto>()!;
                var rawDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                var parsedDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                // 1) map each header → DTO prop (with normalize for Activate)
                foreach (var kv in headerToPropertyMap)
                {
                    var header = kv.Key;
                    var propName = kv.Value;

                    if (!table.Columns.Contains(header))
                        continue;
                    if (!props.TryGetValue(propName, out var prop))
                        continue;

                    var raw = dr[header]?.ToString()?.Trim() ?? "";
                    rawDict[header] = raw;

                    object value;
                    if (propName == nameof(ImportUserDto.CompanyId))
                    {
                        
                        value = companyLookup.TryGetValue(raw, out var id) ? id : 0;
                    }
                    else if (propName == nameof(ImportUserDto.Activate))
                    {
                        // normalize True/TRUE/false → "true"/"false"
                        value = raw.ToLowerInvariant();
                    }
                    else if (propName == nameof(ImportUserDto.LocationCode))
                    {
                        value = raw;
                    }
                    else if (prop.PropertyType == typeof(int))
                    {
                        value = int.TryParse(raw, out var iv) ? iv : 0;
                    }
                    else
                    {
                        value = raw;
                    }

                    var parsed = value?.ToString() ?? "";
                    parsedDict[propName] = parsed;

                    _logger.LogInformation(
                        "Row {RowIndex}, Col '{Header}' → {Prop} raw='{Raw}' parsed='{Parsed}'",
                        rowIndex, header, propName, raw, parsed);

                    prop.SetValue(dto, value);
                }

                // 2) attribute‐level + IValidatableObject.Validate (called automatically)
                var ctx = new ValidationContext(dto, _provider, items: null);
                var vr = new List<ValidationResult>();
                Validator.TryValidateObject(dto, ctx, vr, validateAllProperties: true);

                // 3) gather into result
                results.Add(new RowValidationResult
                {
                    Row = rowIndex,
                    IsValid = vr.Count == 0,
                    Errors = vr.Select(r => r.ErrorMessage!).ToArray(),
                    MemberNames = vr.SelectMany(r => r.MemberNames).Distinct().ToArray(),
                    RawValues = rawDict,
                    ParsedValues = parsedDict
                });
            }

            return results;
        }
    }
}
