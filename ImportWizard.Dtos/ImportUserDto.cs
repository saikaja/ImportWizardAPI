// ImportWizard.Dtos/Models/ImportUserDto.cs

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
        [Required(ErrorMessage = "First Name is required")]
        [RegularExpression("^[A-Za-z0-9]+$", ErrorMessage = "First Name must be alphanumeric")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "First Name must be between 1 and 50 characters")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last Name is required")]
        [RegularExpression("^[A-Za-z0-9]+$", ErrorMessage = "Last Name must be alphanumeric")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Last Name must be between 2 and 100 characters")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Employee Id is required")]
        [RegularExpression("^[A-Za-z0-9]+$", ErrorMessage = "Employee Id must be alphanumeric")]
        [StringLength(50, ErrorMessage = "Employee Id must be at most 50 characters")]
        public string EmployeeId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(50, ErrorMessage = "Email must be at most 50 characters")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role is required")]
        [AllowedRoles]
        public string Role { get; set; } = string.Empty;

        [Required(ErrorMessage = "Printer is required")]
        [RegularExpression("^[A-Za-z0-9]+$", ErrorMessage = "Printer must be alphanumeric")]
        public string Printer { get; set; } = string.Empty;

        [Required(ErrorMessage = "Activate is required")]
        [RegularExpression("^(true|false)$", ErrorMessage = "Activate must be 'true' or 'false'")]
        public string Activate { get; set; } = string.Empty;

        [Required(ErrorMessage = "Company ID is required")]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "Location code is required")]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "Location code must be exactly 4 characters")]
        public string LocationCode { get; set; } = string.Empty;

        // Optional free‐form comments
        public string Comments { get; set; } = string.Empty;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Resolve the AppDbContext from DI
            var db = validationContext.GetService<AppDbContext>()
                     ?? throw new InvalidOperationException("AppDbContext not available in ValidationContext.");

            // 1) Company must exist
            var companyExists = db.Companies.Any(c => c.CompanyId == CompanyId);
            if (!companyExists)
            {
                yield return new ValidationResult(
                    $"Company with ID {CompanyId} does not exist.",
                    new[] { nameof(CompanyId) }
                );
            }

            // 2) LocationCode must belong to that company
            var locationMatches = db.Locations.Any(l =>
                l.CompanyId == CompanyId &&
                l.LocationCode == LocationCode
            );
            if (!locationMatches)
            {
                yield return new ValidationResult(
                    $"Location code '{LocationCode}' is not valid for company {CompanyId}.",
                    new[] { nameof(LocationCode) }
                );
            }

            // no 'yield break' here — both errors will be returned if both checks fail
        }
    }
}
