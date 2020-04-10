using System;
using System.Text.Json;
using System.Threading.Tasks;
using DNTFrameworkCore.Application.Models;
using DNTFrameworkCore.Querying;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DNTFrameworkCore.Web.ModelBinders
{
    public static class FilteredPagedRequestModelBinderExtensions
    {
        public static MvcOptions UseDefaultFilteredPagedRequestModelBinder(this MvcOptions options)
        {
            return options.UseFilteredPagedRequestModelBinder<FilteredPagedRequestModel>();
        }

        public static MvcOptions UseFilteredPagedRequestModelBinder<TFilteredPagedRequestModel>(this MvcOptions options)
            where TFilteredPagedRequestModel : IFilteredPagedRequest
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            options.ModelBinderProviders.Insert(0,
                new FilteredPagedRequestModelBinderProvider<TFilteredPagedRequestModel>());

            return options;
        }
    }

    public class FilteredPagedRequestModelBinderProvider<TFilteredPagedRequestModel> : IModelBinderProvider
        where TFilteredPagedRequestModel : IFilteredPagedRequest
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata == null) return null;

            if (context.Metadata.IsComplexType && context.Metadata.ModelType == typeof(TFilteredPagedRequestModel))
            {
                return new FilteredPagedRequestModelBinder<TFilteredPagedRequestModel>();
            }

            return null;
        }
    }

    public class FilteredPagedRequestModelBinder<TFilteredPagedRequestModel> : IModelBinder
        where TFilteredPagedRequestModel : IFilteredPagedRequest
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

            var model = JsonSerializer.Deserialize<TFilteredPagedRequestModel>(jsonString);

            bindingContext.Result = ModelBindingResult.Success(model);
            return Task.CompletedTask;
        }
    }
}