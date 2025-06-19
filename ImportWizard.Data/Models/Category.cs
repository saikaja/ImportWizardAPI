// Models/Category.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImportWizard.Data.Models
{
    [Table("Category", Schema = "imp")]
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("CategoryId")]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("Name")]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(255)]
        [Column("Description")]
        public string Description { get; set; }


    }
}
