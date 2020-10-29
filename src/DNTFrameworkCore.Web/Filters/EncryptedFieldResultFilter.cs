using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Linq;
using DNTFrameworkCore.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace DNTFrameworkCore.Web.Filters
{
    /// <summary>
    /// for more info see: https://www.dotnettips.info/post/3246
    /// </summary>
    public class EncryptedFieldResultFilter : ResultFilterAttribute
    {
        private readonly IProtectionService _protection;
        private readonly ILogger<EncryptedFieldResultFilter> _logger;
        private readonly ConcurrentDictionary<Type, bool> _models = new ConcurrentDictionary<Type, bool>();

        public EncryptedFieldResultFilter(
            IProtectionService protection,
            ILogger<EncryptedFieldResultFilter> logger)
        {
            _protection = protection;
            _logger = logger;
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            var model = context.Result switch
            {
                PageResult pageResult => pageResult.Model,
                ViewResult viewResult => viewResult.Model,
                PartialViewResult partialViewResult => partialViewResult.Model,
                ObjectResult objectResult => objectResult.Value,
                _ => null
            };

            if (model is null) return;

            if (typeof(IEnumerable).IsAssignableFrom(model.GetType()))
            {
                foreach (var item in model as IEnumerable)
                {
                    Encrypt(item);
                }
            }
            else
            {
                Encrypt(model);
            }
        }

        private void Encrypt(object model)
        {
            var modelType = model.GetType();
            if (_models.TryGetValue(modelType, out var shouldEncrypt) && !shouldEncrypt)
            {
                return;
            }

            foreach (var property in modelType.GetProperties())
            {
                var attribute = property.GetCustomAttributes(typeof(EncryptedFieldAttribute), false).FirstOrDefault();
                if (attribute == null)
                {
                    continue;
                }

                shouldEncrypt = true;

                var value = property.GetValue(model);
                if (value is null)
                {
                    continue;
                }

                if (value.GetType() != typeof(string))
                {
                    _logger.LogWarning(
                        $"[EncryptedField] should be applied to `string` properties, But type of `{property.DeclaringType}.{property.Name}` is `{property.PropertyType}`.");
                    continue;
                }

                var encryptedValue = _protection.Encrypt(value.ToString());
                property.SetValue(model, encryptedValue);
            }

            _models.TryAdd(modelType, shouldEncrypt);
        }
    }
}