using System;
using System.Collections.Generic;
using System.Reflection;
using DNTFrameworkCore.GuardToolkit;

namespace DNTFrameworkCore.Validation
{
    public abstract class ModelValidator<TModel> : IModelValidator<TModel>
    {
        IEnumerable<ValidationFailure> IModelValidator.Validate(object model)
        {
            Guard.ArgumentNotNull(model, nameof(model));

            if (!((IModelValidator) this).CanValidateInstancesOfType(model.GetType()))
            {
                throw new InvalidOperationException(
                    $"Cannot validate instances of type '{model.GetType().Name}'. This validator can only validate instances of type '{typeof(TModel).Name}'.");
            }

            return Validate((TModel) model);
        }

        bool IModelValidator.CanValidateInstancesOfType(Type type)
        {
            return typeof(TModel).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
        }

        public abstract IEnumerable<ValidationFailure> Validate(TModel model);
    }
}