using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DNTFrameworkCore.Application.Models;
using DNTFrameworkCore.Localization;

namespace DNTFrameworkCore.TestWebApp.Application.Blogging.Models
{
    [LocalizationResource(Name = "SharedResource", Location = "DNTFrameworkCore.TestWebApp")]
    public class BlogModel : MasterModel, IValidatableObject
    {
        [Required] public string Title { get; set; }

        [MaxLength(50, ErrorMessage = "Maximum length is 50")]
        [Required]
        public string Url { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Title == "BlogTitle")
            {
                yield return new ValidationResult("IValidatableObject Message", new[] {nameof(Title)});
            }
        }
    }
}