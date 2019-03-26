using System;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DNTFrameworkCore.Web.Mvc.TagHelpers
{
    [HtmlTargetElement("a", Attributes = ModalLinkAttributeName)]
    public class ModalLinkTagHelper : TagHelper
    {
        private const string ModalLinkAttributeName = "modal-link";

        [HtmlAttributeName(ModalLinkAttributeName)]
        public string Url { get; set; }
        public ModalLinkTagHelper() : base()
        {
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.Add("role", "button");
            output.Attributes.Add("data-ajax", "true");
            output.Attributes.Add("data-ajax-method", "GET");
            output.Attributes.Add("data-ajax-url", Url);
            output.Attributes.Add("data-ajax-cache", "false");
            output.Attributes.Add("data-ajax-success", "handleModalLinkLoaded");
            output.Attributes.Add("data-ajax-failure", "handleModalLinkFailed");
            output.Attributes.Add("data-ajax-update", "#main-modal div.modal-content");
            output.Attributes.Add("data-toggle", "modal");
            output.Attributes.Add("data-target", "#main-modal");
        }
    }
}