using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using DNTFrameworkCore.Validation;
using DNTFrameworkCore.Validation.Interception;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Shouldly;

namespace DNTFrameworkCore.Tests.Validation
{
    [TestFixture]
    public class MethodInvocationValidatorTests
    {
        [Test]
        public void Should_Has_Failures_When_Validate_With_Parameter_Implement_IModelValidator()
        {
            var invocationValidator = BuildMethodInvocationValidator(options => { });
            var failures =
                invocationValidator.Validate(
                    typeof(TestType).GetMethod(nameof(TestType.PublicMethodValidableObjectParameter)),
                    new object[] {new TestParameter()}).ToList();
            failures.ShouldNotBeEmpty();
            failures.Any(result => result.MemberName == "TestMember" && result.Message == nameof(IModelValidator));
        }

        [Test]
        public void Should_Has_Failures_When_Validate_With_Parameter_Implement_IValidatableObject()
        {
            var invocationValidator = BuildMethodInvocationValidator(options => { });
            var failures =
                invocationValidator.Validate(
                    typeof(TestType).GetMethod(nameof(TestType.PublicMethodValidableObjectParameter)),
                    new object[] {new TestParameter()}).ToList();
            failures.ShouldNotBeEmpty();
            failures.Any(result => result.MemberName == "TestMember" && result.Message == nameof(IValidatableObject));
        }

        [Test]
        public void Should_Has_Failures_When_Method_With_Multiple_Parameters()
        {
            var invocationValidator = BuildMethodInvocationValidator(options => { });
            var failures =
                invocationValidator.Validate(
                    typeof(TestType).GetMethod(nameof(TestType.PublicMethodWithMultipleParameter)),
                    new object[] {new TestParameter(), new TestParameter2()}).ToList();
            failures.ShouldNotBeEmpty();
            failures.Any(result => result.MemberName == nameof(TestParameter.Property1));
            failures.Any(result => result.MemberName == nameof(TestParameter2.Property2));
        }

        [Test]
        public void Should_Has_Failures_When_Method_With_EnableValidation_Is_In_Type_With_SkipValidation()
        {
            var invocationValidator = BuildMethodInvocationValidator(options => { });
            var failures =
                invocationValidator.Validate(
                    typeof(SkipValidationTestType).GetMethod(nameof(SkipValidationTestType.PublicMethod)),
                    new[] {new TestParameter()}).ToList();
            failures.ShouldNotBeEmpty();
            failures.Any(result => result.MemberName == nameof(TestParameter.Property1));
        }

        [Test]
        public void Should_Not_Has_Failures_When_Method_Has_SkipValidation()
        {
            var invocationValidator = BuildMethodInvocationValidator(options => { });
            var failures =
                invocationValidator.Validate(
                    typeof(TestType).GetMethod(nameof(TestType.SkipValidationPublicMethod)),
                    new[] {new TestParameter()});
            failures.ShouldBeEmpty();
        }

        [Test]
        public void Should_Not_Has_Failures_When_Method_Is_Not_Public()
        {
            var invocationValidator = BuildMethodInvocationValidator(options => { });
            var failures =
                invocationValidator.Validate(
                    typeof(TestType).GetMethod(nameof(TestType.InternalMethod),
                        BindingFlags.NonPublic | BindingFlags.Instance),
                    new[] {new TestParameter()});
            failures.ShouldBeEmpty();
        }

        [Test]
        public void Should_Not_Has_Failures_When_Method_Parameters_Empty()
        {
            var invocationValidator = BuildMethodInvocationValidator(options => { });
            var failures =
                invocationValidator.Validate(typeof(TestType).GetMethod(nameof(TestType.PublicMethod)),
                    Enumerable.Empty<object>().ToArray());
            failures.ShouldBeEmpty();
        }

        private static MethodInvocationValidator BuildMethodInvocationValidator(Action<ValidationOptions> options)
        {
            var services = new ServiceCollection();

            services.AddTransient<IModelValidator<TestParameter>, TestParameterValidator>();
            
            services.AddDNTFrameworkCore()
                .AddModelValidation(options);

            var helper = services.BuildServiceProvider().GetRequiredService<MethodInvocationValidator>();
            return helper;
        }

        public class TestParameter : IValidatableObject
        {
            [Required] public string Property1 { get; set; }

            public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
            {
                yield return new ValidationResult(nameof(IValidatableObject), new[] {"TestMember"});
            }
        }

        public class TestParameterValidator : ModelValidator<TestParameter>
        {
            public override IEnumerable<ValidationFailure> Validate(TestParameter model)
            {
                yield return new ValidationFailure("TestMember", nameof(IModelValidator));
            }
        }

        public class TestParameter2
        {
            [Required] public string Property2 { get; set; }
        }

        public class TestType
        {
            public void PublicMethod()
            {
            }

            public void PublicMethodWithMultipleParameter(TestParameter parameter1, TestParameter2 parameter2)
            {
            }

            public void PublicMethodValidableObjectParameter(TestParameter parameter)
            {
            }

            public void PublicMethodModelValidatorParameter(TestParameter parameter)
            {
            }

            internal void InternalMethod(TestParameter parameter)
            {
            }

            [SkipValidation]
            public void SkipValidationPublicMethod(TestParameter parameter)
            {
            }
        }

        [SkipValidation]
        public class SkipValidationTestType
        {
            [EnableValidation]
            public void PublicMethod(TestParameter parameter)
            {
            }
        }
    }
}