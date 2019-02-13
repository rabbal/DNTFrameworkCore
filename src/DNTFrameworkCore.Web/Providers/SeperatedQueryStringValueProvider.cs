using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;

namespace DNTFrameworkCore.Web.Providers
{
    public class SeparatedQueryStringValueProvider : QueryStringValueProvider
    {
        private readonly string _key;
        private readonly string _separator;
        private readonly IQueryCollection _values;

        public SeparatedQueryStringValueProvider(IQueryCollection values, string separator)
            : this(null, values, separator)
        {
        }

        public SeparatedQueryStringValueProvider(string key, IQueryCollection values, string separator)
            : base(BindingSource.Query, values, CultureInfo.InvariantCulture)
        {
            _key = key;
            _values = values;
            _separator = separator;
        }

        public override ValueProviderResult GetValue(string key)
        {
            var result = base.GetValue(key);

            if (_key != null && _key != key)
            {
                return result;
            }

            if (result != ValueProviderResult.None &&
                result.Values.Any(x => x.IndexOf(_separator, StringComparison.OrdinalIgnoreCase) > 0))
            {
                var splitValues = new StringValues(result.Values
                    .SelectMany(x => x.Split(new[] {_separator}, StringSplitOptions.None)).ToArray());
                return new ValueProviderResult(splitValues, result.Culture);
            }

            return result;
        }
    }

    public class SeparatedQueryStringValueProviderFactory : IValueProviderFactory
    {
        private readonly string _separator;
        private readonly string _key;

        public SeparatedQueryStringValueProviderFactory(string separator) : this(null, separator)
        {
        }

        public SeparatedQueryStringValueProviderFactory(string key, string separator)
        {
            _key = key;
            _separator = separator;
        }

        public Task CreateValueProviderAsync(ValueProviderFactoryContext context)
        {
            context.ValueProviders.Insert(0,
                new SeparatedQueryStringValueProvider(_key, context.ActionContext.HttpContext.Request.Query,
                    _separator));
            return Task.CompletedTask;
        }
    }
}