using System.Linq;
using DNTFrameworkCore.Validation;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Shouldly;

namespace DNTFrameworkCore.FluentValidation.Tests
{
    [TestFixture]
    public class FluentValidationModelValidatorTests
    {
        [Test]
        public void Should_Have_Failures_When_Validator_Resolved_And_Defined_Some_Rules()
        {
            var services = new ServiceCollection();
            services.AddDNTFrameworkCore()
                .AddFluentModelValidation();

            services.AddSingleton<IValidatorFactory, ServiceProviderValidatorFactory>();
            services.AddTransient<IValidator<TestModel>, TestModelValidator>();

            var validator = services.BuildServiceProvider().GetRequiredService<IModelValidator<TestModel>>();

            validator.ShouldNotBeNull();
            var failures = validator.Validate(new TestModel());

            failures.Any(result => result.Message == nameof(TestModelValidator)).ShouldBeTrue();
        }

        [Test]
        public void Should_Not_Have_Failures_When_Validator_Not_Registered()
        {
            var services = new ServiceCollection();
            services.AddDNTFrameworkCore()
                .AddFluentModelValidation();

            services.AddSingleton<IValidatorFactory, ServiceProviderValidatorFactory>();
            // services.AddTransient<IValidator<TestModel>, TestModelValidator>();

            var validator = services.BuildServiceProvider().GetRequiredService<IModelValidator<TestModel>>();

            validator.ShouldNotBeNull();
            var failures = validator.Validate(new TestModel());

            failures.ShouldBeEmpty();
        }

        public class TestModel
        {
        }

        public class TestModelValidator : FluentModelValidator<TestModel>
        {
            public TestModelValidator()
            {
                RuleFor(m => m).Custom((model, context) => { context.AddFailure(nameof(TestModelValidator)); });
            }
        }
    }
}