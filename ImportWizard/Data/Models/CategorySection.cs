using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImportWizard.Data.Models
{
    [Table("CategorySection", Schema = "imp")]
    public class CategorySection
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("SectionId")]
        public int SectionId { get; set; }

        [Required]
        [ForeignKey(nameof(Category))]
        [Column("CategoryId")]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("SectionName")]
        public string SectionName { get; set; } = null!;

        [Required]
        [MaxLength(255)]
        [Column("SectionDescription")]
        public string SectionDescription { get; set; }

        [Required]
        [Column("IsActive")]
        public bool IsActive { get; set; } = true;
    }
}