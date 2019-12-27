using DNTFrameworkCore.FluentValidation;
using DNTFrameworkCore.TestTenancy.Application.Blogging.Models;
using DNTFrameworkCore.TestTenancy.Resources;

namespace DNTFrameworkCore.TestTenancy.Application.Blogging.Validators
{
    public class BlogValidator : FluentModelValidator<BlogModel>
    {
        public BlogValidator(IMessageLocalizer localizer)
        {
            RuleFor(b => b.Title).NotEmpty()
                .WithMessage(localizer["Blog.Fields.Title.Required"]);
        }
    }
}