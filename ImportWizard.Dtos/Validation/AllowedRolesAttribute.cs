// ImportWizard.Dtos/Validation/AllowedRolesAttribute.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.Extensions.Options;

namespace ImportWizard.Dtos.Validation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class AllowedRolesAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            // Grab the RolesConfig from DI
            var opts = context.GetService(typeof(IOptions<RolesConfig>))
                       as IOptions<RolesConfig>;
            var allowed = opts?.Value.AllowedRoles
                          ?? Array.Empty<string>();

            var str = (value as string ?? "")
                        .Trim()
                        .ToLowerInvariant();

            if (allowed.Any(r => string.Equals(r, str, StringComparison.OrdinalIgnoreCase)))
            {
                return ValidationResult.Success!;
            }

            return new ValidationResult(
                $"The field {context.MemberName} must be one of: {string.Join(", ", allowed)}"
            );
        }
    }
}
