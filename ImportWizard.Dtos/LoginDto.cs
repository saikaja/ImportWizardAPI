using System.ComponentModel.DataAnnotations;

namespace ImportWizard.Dtos
{
    public class LoginDto
    {
        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }
}
