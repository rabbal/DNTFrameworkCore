using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DNTFrameworkCore.Application;
using DNTFrameworkCore.Localization;
using DNTFrameworkCore.Querying;

namespace DNTFrameworkCore.TestAPI.Application.Blogging.Models
{
    [LocalizationResource(Name = "SharedResource", Location = "DNTFrameworkCore.TestAPI")]
    public class BlogModel : MasterModel, IValidatableObject
    {
        [QueryField(Sorting = true, Filtering = true)]
        public string Title { get; set; }

        [MaxLength(50, ErrorMessage = "Maximum length is 50")]
        public string Url { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Title == "BlogTitle")
            {
                yield return new ValidationResult("IValidatableObject Message", new[] { nameof(Title) });
                yield return new ValidationResult("IValidatableObject Message");
            }
        }
    }
}