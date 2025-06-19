namespace ImportWizard.Dtos
{
    public class LocationDto
    {
        public int LocationId { get; set; }
        public int CompanyId { get; set; }
        public string LocationCode { get; set; } = string.Empty;
        public string LocationAddress { get; set; } = string.Empty;
    }
}
