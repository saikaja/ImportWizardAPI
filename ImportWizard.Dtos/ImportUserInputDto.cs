// File: ImportWizard.Dtos/ImportUserInputDto.cs
namespace ImportWizard.Dtos
{
    public class ImportUserInputDto
    {
        public string Company { get; set; } = string.Empty;  // From Excel “Company” column
        public string LocationCode { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string EmployeeId { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Printer { get; set; } = string.Empty;
        public string Activate { get; set; } = string.Empty;
        public string Comments { get; set; } = string.Empty;
    }
}
