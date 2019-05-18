using System;
using System.Collections.Generic;

namespace DNTFrameworkCore.Validation
{
    public interface IModelValidator<in TModel> : IModelValidator
    {
        /// <summary>
        /// Validate the specified instance synchronously.
        /// contains validation logic and business rules validation
        /// </summary>
        /// <param name="model">model to validate</param>
        /// <returns>
        /// A list of <see cref="ValidationFailure"/> indicating the results of validating the model value.
        /// </returns>
        IEnumerable<ValidationFailure> Validate(TModel model);
    }

    public interface IModelValidator
    {
        /// <summary>
        /// Validate the specified instance synchronously.
        /// contains validation logic and business rules validation
        /// </summary>
        /// <param name="model">model to validate</param>
        /// <returns>
        /// A list of <see cref="ValidationFailure"/> indicating the results of validating the model value.
        /// </returns>
        IEnumerable<ValidationFailure> Validate(object model);

        bool CanValidateInstancesOfType(Type type);
    }
}