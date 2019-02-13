using DNTFrameworkCore.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore.FluentValidation
{
    public static class Extensions
    {
        public static IDNTBuilder AddFluentModelValidation(this IDNTBuilder builder)
        {
            builder.Services.AddTransient(typeof(IModelValidator<>), typeof(FluentValidationModelValidator<>));
            return builder;
        }
    }
}