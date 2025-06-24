namespace ImportWizard.Dtos
{
    public class ImportResultDto
    {
        public string Email { get; set; } = string.Empty;
        public bool Inserted { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
