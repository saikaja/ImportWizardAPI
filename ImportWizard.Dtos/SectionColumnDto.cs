namespace ImportWizard.Dtos
{
    public class SectionColumnDto
    {
        public int ColumnId { get; set; }
        public int SectionId { get; set; }
        public string ColumnName { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public string DataType { get; set; } = null!;
        public string DbColumnName { get; set; }
        public bool IsIdentifier { get; set; }
        public OptionsDto? Options { get; set; }
    }
}
