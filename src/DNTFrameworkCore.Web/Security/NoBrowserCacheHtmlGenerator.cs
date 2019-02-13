using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace DNTFrameworkCore.Web.Security
{
    public class NoBrowserCacheHtmlGenerator : DefaultHtmlGenerator
    {
        public NoBrowserCacheHtmlGenerator(
            IAntiforgery antiforgery,
            IOptions<MvcViewOptions> optionsAccessor,
            IModelMetadataProvider metadataProvider,
            IUrlHelperFactory urlHelperFactory,
            HtmlEncoder htmlEncoder,
            ValidationHtmlAttributeProvider validationAttributeProvider)
            : base(
                antiforgery,
                optionsAccessor,
                metadataProvider,
                urlHelperFactory,
                htmlEncoder,
                validationAttributeProvider)
        {
        }

        public override IHtmlContent GenerateAntiforgery(ViewContext viewContext)
        {
            var result = base.GenerateAntiforgery(viewContext);

            // disable caching for the browser back button
            viewContext
                    .HttpContext
                    .Response
                    .Headers[HeaderNames.CacheControl]
                = "no-cache, max-age=0, must-revalidate, no-store";

            return result;
        }
    }
}