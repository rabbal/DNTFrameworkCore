using DNTFrameworkCore.FluentValidation;
using DNTFrameworkCore.TestWebApp.Application.Blogging.Models;
using DNTFrameworkCore.TestWebApp.Resources;
using FluentValidation;

namespace DNTFrameworkCore.TestWebApp.Application.Blogging.Validators
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