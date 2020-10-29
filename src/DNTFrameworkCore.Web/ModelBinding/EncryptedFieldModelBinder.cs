using System;
using System.Linq;
using System.Threading.Tasks;
using DNTFrameworkCore.Cryptography;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DNTFrameworkCore.Web.ModelBinding
{
    /// <summary>
    /// for more info see: https://www.dotnettips.info/post/3246
    /// </summary>
    public class EncryptedFieldModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.IsComplexType)
            {
                return null;
            }

            var propertyName = context.Metadata.PropertyName;
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                return null;
            }

            var propertyInfo = context.Metadata.ContainerType.GetProperty(propertyName);
            if (propertyInfo == null)
            {
                return null;
            }

            var attribute = propertyInfo.GetCustomAttributes(typeof(EncryptedFieldAttribute), false).FirstOrDefault();
            return attribute == null ? null : new BinderTypeModelBinder(typeof(EncryptedFieldModelBinder));
        }
    }
    
    public class EncryptedFieldModelBinder : IModelBinder
    {
        private readonly IProtectionService _protection;

        public EncryptedFieldModelBinder(IProtectionService protection)
        {
            _protection = protection;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var logger = bindingContext.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();
            var fallbackBinder = new SimpleTypeModelBinder(bindingContext.ModelType, logger);
            
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueProviderResult == ValueProviderResult.None)
            {
                return fallbackBinder.BindModelAsync(bindingContext);
            }

            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

            var stringValue = valueProviderResult.FirstValue;
            if (string.IsNullOrWhiteSpace(stringValue))
            {
                return fallbackBinder.BindModelAsync(bindingContext);
            }

            var decryptedResult = _protection.Decrypt(stringValue);
            bindingContext.Result = ModelBindingResult.Success(decryptedResult);
            return Task.CompletedTask;
        }
    }
}