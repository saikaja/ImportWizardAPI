using System;
using System.ComponentModel.DataAnnotations;

namespace ImportWizard.Data.Models
{
    public class ImportMaster
    {
        [Key]
        public int ImportId { get; set; }

        public string FileName { get; set; }
        public DateTime SubmittedAt { get; set; }
        public string Status { get; set; }
    }
}
