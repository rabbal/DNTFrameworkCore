using System.Collections.Generic;
using DNTFrameworkCore.Dependency;

namespace DNTFrameworkCore.Validation.Interception
{
    /// <summary>
    /// This interface is used to validate method parameters.
    /// </summary>
    public interface IMethodParameterValidator : ITransientDependency
    {
        IEnumerable<ModelValidationResult> Validate(object parameter);
    }
}