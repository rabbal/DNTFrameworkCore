using System;
using System.Text.Json;
using System.Threading.Tasks;
using DNTFrameworkCore.Querying;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DNTFrameworkCore.Web.ModelBinding
{
    public static class FilteredPagedRequestBinderExtensions
    {
        public static MvcOptions UseFilteredPagedRequestModelBinder(this MvcOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            options.ModelBinderProviders.Insert(0, new FilteredPagedRequestModelBinderProvider());

            return options;
        }
    }

    public class FilteredPagedRequestModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata == null) return null;

            if (context.Metadata.IsComplexType &&
                typeof(IFilteredPagedRequest).IsAssignableFrom(context.Metadata.ModelType))
            {
                return new FilteredPagedRequestModelBinder();
            }

            return null;
        }
    }

    public class FilteredPagedRequestModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

            var jsonString = valueProviderResult.FirstValue;
            if (string.IsNullOrWhiteSpace(jsonString))
            {
                return Task.CompletedTask;
            }

            var model = JsonSerializer.Deserialize(jsonString, bindingContext.ModelType);

            bindingContext.Result = ModelBindingResult.Success(model);
            return Task.CompletedTask;
        }
    }
}