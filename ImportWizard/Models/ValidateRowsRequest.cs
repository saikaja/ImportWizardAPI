// File: ImportWizard.WebApi/Models/ValidateRowsRequest.cs

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ImportWizard.WebApi.Models
{
    public class ValidateRowsRequest
    {
        [Required]
        [FromForm(Name = "file")]
        public IFormFile File { get; set; } = default!;

        [Required]
        [FromForm(Name = "mappings")]
        public string Mappings { get; set; } = default!;

        public Dictionary<string, string> GetMappingDictionary()
        {
            if (string.IsNullOrWhiteSpace(Mappings))
                throw new InvalidOperationException("Mappings JSON is empty.");

            return System.Text.Json.JsonSerializer
                   .Deserialize<Dictionary<string, string>>(Mappings)
                   ?? throw new InvalidOperationException("Unable to parse mappings JSON.");
        }
    }
}
