using System;

namespace ImportWizard.Dtos
{
    public class SaveTemplateDto
    {
        public int TemplateId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
