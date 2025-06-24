// ImportWizard.Data/Models/Template.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImportWizard.Data.Models
{
    [Table("Templates", Schema = "rd")]
    public class Template
    {
        [Key]
        public int TemplateId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [Required]
        public byte[] FileBytes { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
