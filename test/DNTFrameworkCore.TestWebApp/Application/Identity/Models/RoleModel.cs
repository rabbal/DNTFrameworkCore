using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DNTFrameworkCore.Application.Models;

namespace DNTFrameworkCore.TestWebApp.Application.Identity.Models
{
    public class RoleModel : MasterModel<long>, IValidatableObject
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<PermissionModel> Permissions { get; set; } = new HashSet<PermissionModel>();
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Name == "Admin")
            {
                yield return new ValidationResult("IValidatableObject Message", new[] { nameof(Name) });
            }

            if (Name == "Admin2")
            {
                yield return new ValidationResult("IValidatableObject Message");
            }
        }
    }
}