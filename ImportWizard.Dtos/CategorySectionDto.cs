namespace ImportWizard.Dtos
{
    public class CategorySectionDto
    {
        public int SectionId { get; set; }
        public int CategoryId { get; set; }
        public string SectionName { get; set; } = null!;
        public string SectionDescription { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
