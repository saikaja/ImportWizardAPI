// File: ImportWizard.Dtos/Validation/RowValidationResult.cs
using System;
using System.Collections.Generic;

namespace ImportWizard.Dtos.Validation
{
    public class RowValidationResult
    {
        /// <summary>Zero-based row index in the spreadsheet.</summary>
        public int Row { get; set; }

        /// <summary>True if no validation errors.</summary>
        public bool IsValid { get; set; }

        /// <summary>All error messages for this row.</summary>
        public string[] Errors { get; set; } = Array.Empty<string>();

        /// <summary>
        /// The exact DTO-property names (e.g. "FirstName", "CompanyName", "LocationCode")
        /// that triggered each error. Used for precise highlighting.
        /// </summary>
        public string[] MemberNames { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Raw cell values read from Excel, keyed by column header.
        /// </summary>
        public Dictionary<string, string> RawValues { get; set; }
            = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Values after parsing/coercion (e.g. CompanyId = 0 or 1, Activate = "true"), 
        /// keyed by DTO property name.
        /// </summary>
        public Dictionary<string, string> ParsedValues { get; set; }
            = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    }
}
