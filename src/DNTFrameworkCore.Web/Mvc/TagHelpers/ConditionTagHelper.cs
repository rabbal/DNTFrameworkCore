using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DNTFrameworkCore.Web.Mvc.TagHelpers
{
    [HtmlTargetElement(Attributes = ConditionAttributeName)]
    public class ConditionTagHelper : TagHelper
    {
        private const string ConditionAttributeName = "condition";
        [HtmlAttributeName(ConditionAttributeName)]
        public bool Condition { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!Condition)
            {
                output.SuppressOutput();
            }
        }
    }
}