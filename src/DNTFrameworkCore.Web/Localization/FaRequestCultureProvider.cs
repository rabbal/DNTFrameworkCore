using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;

namespace DNTFrameworkCore.Web.Localization
{
    public class FaRequestCultureProvider : RequestCultureProvider
    {
        public override Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            return Task.FromResult(new ProviderCultureResult("fa-IR"));
        }
    }
}