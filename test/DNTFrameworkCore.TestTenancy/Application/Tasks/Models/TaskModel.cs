using DNTFrameworkCore.TestTenancy.Domain.Tasks;

namespace DNTFrameworkCore.TestTenancy.Application.Tasks.Models
{
    [LocalizationResource(Name = "SharedResource", Location = "DNTFrameworkCore.TestAPI")]
    public class TaskModel : MasterModel, IValidatableObject
    {
        public string Title { get; set; }

        [MaxLength(50, ErrorMessage = "Validation from DataAnnotations")]
        public string Number { get; set; }

        public string Description { get; set; }
        public TaskState State { get; set; } = TaskState.Todo;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Title == "IValidatableObject")
            {
                yield return new ValidationResult("Validation from IValidatableObject");
            }
        }
    }
}