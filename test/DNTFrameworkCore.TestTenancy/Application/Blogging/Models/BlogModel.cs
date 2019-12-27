namespace DNTFrameworkCore.TestTenancy.Application.Blogging.Models
{
    [LocalizationResource(Name = "SharedResource", Location = "DNTFrameworkCore.TestAPI")]
    public class BlogModel : MasterModel, IValidatableObject
    {
        public string Title { get; set; }

        [MaxLength(50, ErrorMessage = "Maximum length is 50")]
        public string Url { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Title == "BlogTitle")
            {
                yield return new ValidationResult("IValidatableObject Message", new[] {nameof(Title)});
                yield return new ValidationResult("IValidatableObject Message");
            }
        }
    }
}