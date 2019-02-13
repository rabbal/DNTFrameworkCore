using System;
using System.Threading.Tasks;
using DNTFrameworkCore.Application.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace DNTFrameworkCore.Web.ModelBinders
{
    public static class FilteredPagedQueryModelBinderExtensions
    {
        public static MvcOptions UseDefaultFilteredPagedQueryModelBinder(this MvcOptions options)
        {
            return options.UseFilteredPagedQueryModelBinder<FilteredPagedQueryModel>();
        }

        public static MvcOptions UseFilteredPagedQueryModelBinder<TFilteredPagedQueryModel>(this MvcOptions options)
            where TFilteredPagedQueryModel : IFilteredPagedQueryModel
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            options.ModelBinderProviders.Insert(0,
                new FilteredPagedQueryModelBinderProvider<TFilteredPagedQueryModel>());

            return options;
        }
    }

    public class FilteredPagedQueryModelBinderProvider<TFilteredPagedQueryModel> : IModelBinderProvider
        where TFilteredPagedQueryModel : IFilteredPagedQueryModel
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata == null) return null;

            if (context.Metadata.IsComplexType && context.Metadata.ModelType == typeof(TFilteredPagedQueryModel))
            {
                return new FilteredPagedQueryModelBinder<TFilteredPagedQueryModel>();
            }

            return null;
        }
    }

    public class FilteredPagedQueryModelBinder<TFilteredPagedQueryModel> : IModelBinder
        where TFilteredPagedQueryModel : IFilteredPagedQueryModel
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

            var model = JsonConvert.DeserializeObject<TFilteredPagedQueryModel>(jsonString);

            bindingContext.Result = ModelBindingResult.Success(model);
            return Task.CompletedTask;
        }
    }
}