namespace ImportWizard.Dtos
{
    public class CategoryHierarchyDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public List<SectionHierarchyDto> Sections { get; set; } = new();
    }

    public class SectionHierarchyDto
    {
        public int SectionId { get; set; }
        public int CategoryId { get; set; }
        public string SectionName { get; set; } = null!;
        public string SectionDescription { get; set; } = null!;
        public bool IsActive { get; set; }
        // reuse your existing SectionColumnDto
        public List<SectionColumnDto> Columns { get; set; } = new();
    }
}
