// ImportWizard.Dtos/Validation/RolesConfig.cs
using System;

namespace ImportWizard.Dtos.Validation
{
    /// <summary>
    /// Holds your list of allowed role names, loaded from configuration.
    /// </summary>
    public class RolesConfig
    {
        public string[] AllowedRoles { get; set; } = Array.Empty<string>();
    }
}
