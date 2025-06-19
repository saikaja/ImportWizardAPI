// Models/SectionColumn.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Collections.Specialized.BitVector32;

namespace ImportWizard.Data.Models
{
    [Table("SectionColumn", Schema = "imp")]
    public class SectionColumn
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ColumnId")]
        public int ColumnId { get; set; }

        [Required]
        [ForeignKey(nameof(Section))]
        [Column("SectionId")]
        public int SectionId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("ColumnName")]
        public string ColumnName { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        [Column("DisplayName")]
        public string DisplayName { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        [Column("DataType")]
        public string DataType { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        [Column("DbColumnName")]
        public string DbColumnName { get; set; } = null!;

        [Required]
        [Column("IsIdentifier")]
        public bool IsIdentifier { get; set; }

        [Column("Options")]
        public string? Options { get; set; }
    }
}
