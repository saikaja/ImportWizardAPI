// File: ImportWizard.Dtos/Models/ImportUserDto.cs

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ImportWizard.Data;              // for AppDbContext
using Microsoft.Extensions.DependencyInjection;
using ImportWizard.Dtos.Validation;   // for AllowedRoles attribute

namespace ImportWizard.Dtos
{
    public class ImportUserDto : IValidatableObject
    {
        [Required, RegularExpression("^[A-Za-z0-9]+$"), StringLength(50, MinimumLength = 1)]
        public string FirstName { get; set; } = string.Empty;

        [Required, RegularExpression("^[A-Za-z0-9]+$"), StringLength(100, MinimumLength = 2)]
        public string LastName { get; set; } = string.Empty;

        [Required, RegularExpression("^[A-Za-z0-9]+$"), StringLength(50)]
        public string EmployeeId { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(50)]
        public string Email { get; set; } = string.Empty;

        [Required, AllowedRoles]
        public string Role { get; set; } = string.Empty;

        [Required, RegularExpression("^[A-Za-z0-9]+$")]
        public string Printer { get; set; } = string.Empty;

        [Required, RegularExpression("^(true|false)$")]
        public string Activate { get; set; } = string.Empty;

        // We keep CompanyId for backward‐compatible validations...
        [Required]
        public int CompanyId { get; set; }

        // …but also carry the raw name string
        public string CompanyName { get; set; } = string.Empty;

        [Required, StringLength(4, MinimumLength = 4)]
        public string LocationCode { get; set; } = string.Empty;

        public string Comments { get; set; } = string.Empty;

        public IEnumerable<ValidationResult> Validate(ValidationContext ctx)
        {
            var db = ctx.GetService<AppDbContext>()
                     ?? throw new InvalidOperationException("AppDbContext missing");

            if (!db.Companies.Any(c => c.CompanyId == CompanyId))
                yield return new ValidationResult(
                    $"Company with ID {CompanyId} does not exist.",
                    new[] { nameof(CompanyId) });

            if (!db.Locations.Any(l => l.CompanyId == CompanyId && l.LocationCode == LocationCode))
                yield return new ValidationResult(
                    $"Location code '{LocationCode}' is not valid for company {CompanyId}.",
                    new[] { nameof(LocationCode) });
        }
    }
}
