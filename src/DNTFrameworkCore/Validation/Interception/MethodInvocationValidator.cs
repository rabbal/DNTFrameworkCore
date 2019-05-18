using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using DNTFrameworkCore.Collections;
using DNTFrameworkCore.Dependency;
using DNTFrameworkCore.GuardToolkit;
using DNTFrameworkCore.Normalization;
using DNTFrameworkCore.ReflectionToolkit;
using Microsoft.Extensions.Options;

namespace DNTFrameworkCore.Validation.Interception
{
    /// <summary>
    /// This class is used to validate a method call (invocation) for method arguments.
    /// </summary>
    public class MethodInvocationValidator : ITransientDependency
    {
        private const int MaxRecursiveParameterValidationDepth = 8;

        private readonly IOptions<ValidationOptions> _options;
        private readonly IEnumerable<IMethodParameterValidator> _validators;
        private readonly IList<ValidationFailure> _failures;
        private readonly IList<IShouldNormalize> _objectsToBeNormalized;

        public MethodInvocationValidator(
            IOptions<ValidationOptions> options,
            IEnumerable<IMethodParameterValidator> validators)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _validators = validators ?? throw new ArgumentNullException(nameof(validators));

            _failures = new List<ValidationFailure>();
            _objectsToBeNormalized = new List<IShouldNormalize>();
        }

        public IEnumerable<ValidationFailure> Validate(MethodInfo method, object[] parameterValues)
        {
            Guard.ArgumentNotNull(method, nameof(method));
            Guard.ArgumentNotNull(parameterValues, nameof(parameterValues));

            var parameters = method.GetParameters();

            if (parameters.IsNullOrEmpty())
            {
                return Enumerable.Empty<ValidationFailure>();
            }

            if (!method.IsPublic)
            {
                return Enumerable.Empty<ValidationFailure>();
            }

            if (IsValidationSkipped(method))
            {
                return Enumerable.Empty<ValidationFailure>();
            }

            if (parameters.Length != parameterValues.Length)
            {
                throw new Exception("Method parameter count does not match with argument count!");
            }

            for (var i = 0; i < parameters.Length; i++)
            {
                ValidateMethodParameter(parameters[i], parameterValues[i]);
            }

            if (_failures.Any())
            {
                return _failures;
            }

            foreach (var objectToBeNormalized in _objectsToBeNormalized)
            {
                objectToBeNormalized.Normalize();
            }

            return Enumerable.Empty<ValidationFailure>();
        }

        protected virtual bool IsValidationSkipped(MethodInfo method)
        {
            if (method.IsDefined(typeof(EnableValidationAttribute), true))
            {
                return false;
            }

            return ReflectionHelper
                       .GetSingleAttributeOfMemberOrDeclaringTypeOrDefault<SkipValidationAttribute>(method) != null;
        }

        /// <summary>
        /// Validates given parameter for given value.
        /// </summary>
        /// <param name="parameterInfo">Parameter of the method to validate</param>
        /// <param name="parameterValue">Value to validate</param>
        protected virtual void ValidateMethodParameter(ParameterInfo parameterInfo, object parameterValue)
        {
            if (parameterValue == null)
            {
                if (!parameterInfo.IsOptional &&
                    !parameterInfo.IsOut &&
                    !TypeHelper.IsPrimitiveExtendedIncludingNullable(parameterInfo.ParameterType, true))
                {
                    _failures.Add(new ValidationFailure(parameterInfo.Name, parameterInfo.Name + " is null!"));
                }

                return;
            }

            ValidateObjectRecursively(parameterValue, 1);
        }

        protected virtual void ValidateObjectRecursively(object validatingObject, int depth)
        {
            if (depth > MaxRecursiveParameterValidationDepth)
            {
                return;
            }

            if (validatingObject == null)
            {
                return;
            }

            if (_options.Value.IgnoredTypes.Any(t => t.IsInstanceOfType(validatingObject)))
            {
                return;
            }

            if (TypeHelper.IsPrimitiveExtendedIncludingNullable(validatingObject.GetType()))
            {
                return;
            }

            SetValidationErrors(validatingObject);

            // Validate items of enumerable
            if (IsEnumerable(validatingObject))
            {
                foreach (var item in (IEnumerable) validatingObject)
                {
                    ValidateObjectRecursively(item, depth + 1);
                }
            }

            // Add list to be normalized later
            if (validatingObject is IShouldNormalize shouldNormalize)
            {
                _objectsToBeNormalized.Add(shouldNormalize);
            }

            if (!ShouldMakeDeepValidation(validatingObject)) return;

            var properties = TypeDescriptor.GetProperties(validatingObject).Cast<PropertyDescriptor>();
            foreach (var property in properties)
            {
                if (property.Attributes.OfType<SkipValidationAttribute>().Any())
                {
                    continue;
                }

                ValidateObjectRecursively(property.GetValue(validatingObject), depth + 1);
            }
        }

        protected virtual void SetValidationErrors(object parameter)
        {
            foreach (var validator in _validators)
            {
                if (!ShouldValidateUsingValidator(parameter, validator.GetType())) continue;

                var failures = validator.Validate(parameter);
                _failures.AddRange(failures);
            }
        }

        protected virtual bool ShouldValidateUsingValidator(object validatingObject, Type validatorType)
        {
            return true;
        }

        protected virtual bool ShouldMakeDeepValidation(object validatingObject)
        {
            // Do not recursively validate for enumerable objects
            if (validatingObject is IEnumerable)
            {
                return false;
            }

            var validatingObjectType = validatingObject.GetType();

            // Do not recursively validate for primitive objects
            return !TypeHelper.IsPrimitiveExtendedIncludingNullable(validatingObjectType);
        }

        private static bool IsEnumerable(object validatingObject)
        {
            return
                validatingObject is IEnumerable &&
                !(validatingObject is IQueryable) &&
                !TypeHelper.IsPrimitiveExtendedIncludingNullable(validatingObject.GetType());
        }
    }
}