using System;
using System.Collections.Generic;
using System.Linq;

namespace DNTFrameworkCore.Validation
{
    public class NullModelValidator<TModel> : IModelValidator<TModel>
    {
        public bool CanValidateInstancesOfType(Type type)
        {
            return true;
        }

        public IEnumerable<ValidationFailure> Validate(TModel model)
        {
            return Enumerable.Empty<ValidationFailure>();
        }

        public IEnumerable<ValidationFailure> Validate(object model)
        {
            return Enumerable.Empty<ValidationFailure>();
        }
    }
}
