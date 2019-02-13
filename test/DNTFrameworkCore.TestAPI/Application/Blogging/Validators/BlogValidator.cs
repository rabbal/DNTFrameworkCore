using DNTFrameworkCore.FluentValidation;
using DNTFrameworkCore.TestAPI.Application.Blogging.Models;
using DNTFrameworkCore.TestAPI.Resources;
using FluentValidation;

namespace DNTFrameworkCore.TestAPI.Application.Blogging.Validators
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