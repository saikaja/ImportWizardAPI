using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImportWizard.Data.Models
{
    [Table("Location", Schema = "rd")]
    public class Location
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("LocationId")]
        public int LocationId { get; set; }

        [Required]
        [ForeignKey("CompanyId")]
        [Column("CompanyId")]
        public int CompanyId { get; set; }

        [Required]
        [MaxLength(4)]
        [Column("LocationCode")]
        public string LocationCode { get; set; } = null!;

        [Required]
        [Column("LocationAddress")]
        public string LocationAddress { get; set; } = null!;
    }
}
