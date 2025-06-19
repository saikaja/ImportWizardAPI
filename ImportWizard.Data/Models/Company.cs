using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImportWizard.Data.Models
{
    [Table("Company", Schema = "rd")]
    public class Company
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("CompanyId")]
        public int CompanyId { get; set; }

        [Required]
        [Column("Name")]
        public string Name { get; set; } = null!;

        [Required]
        [Column("Description")]
        public string Description { get; set; } = null!;

        [Required]
        [Column("IsActive")]
        public bool IsActive { get; set; }
    }
}
