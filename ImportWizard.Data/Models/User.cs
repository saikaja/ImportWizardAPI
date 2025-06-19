using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImportWizard.Data.Models
{
    [Table("Users", Schema = "imp")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [MaxLength(100)]              // optional
        public string? Company { get; set; }

        [MaxLength(100)]              // optional


        public string? Location { get; set; }

        [Required]                    // mandatory
        [MaxLength(50)]
        public string FirstName { get; set; } = null!;

        [Required]                    // mandatory
        [MaxLength(50)]
        public string LastName { get; set; } = null!;

        [MaxLength(50)]               // optional
        public string? EmployeeId { get; set; }

        [Required]                    // mandatory
        [MaxLength(200)]
        public string Email { get; set; } = null!;

        [Required]                    // mandatory
        [MaxLength(50)]
        public string Role { get; set; } = null!;

        [Required]                    // mandatory
        [MaxLength(50)]
        public string Printer { get; set; } = null!;

        public bool? ActivateNow { get; set; }  // optional

        [MaxLength(500)]              // optional
        public string? Comments { get; set; }
    }
}
