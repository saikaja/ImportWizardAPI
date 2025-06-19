namespace ImportWizard.Dtos
{
    public class OptionsDto
    {
        /// <summary>
        /// e.g. "string", "number", etc.
        /// </summary>
        public string Type { get; set; } = null!;

        public bool Required { get; set; }
        public int? MaxLength { get; set; }
        public int? MinLength { get; set; }
        public string? Format { get; set; }
        public string? Regex { get; set; }

        /// <summary>
        /// The default value, e.g. "user123"
        /// </summary>
        public string? Default { get; set; }
    }
}
