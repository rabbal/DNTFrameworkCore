using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DNTFrameworkCore.Web.Mvc.TagHelpers
{
    public class IfTagHelper : TagHelper
    {
        public override int Order => -1000;

        [HtmlAttributeName("asp-include-if")]
        public bool Include { get; set; } = true;

        [HtmlAttributeName("asp-exclude-if")]
        public bool Exclude { get; set; } = false;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = null;
            if (!Include || Exclude)
            {
                output.SuppressOutput();
            }
        }
    }
}