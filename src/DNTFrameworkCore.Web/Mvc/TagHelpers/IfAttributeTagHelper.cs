using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DNTFrameworkCore.Web.Mvc.TagHelpers
{
    [HtmlTargetElement(Attributes = IfAttributeName)]
    public class IfAttributeTagHelper : TagHelper
    {
        private const string IfAttributeName = "asp-if";
        public override int Order => -1000;

        [HtmlAttributeName(IfAttributeName)]
        public bool Include { get; set; } = true;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!Include)
            {
                output.SuppressOutput();
            }
        }
    }
}