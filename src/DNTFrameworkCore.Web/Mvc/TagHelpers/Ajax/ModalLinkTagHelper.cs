using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DNTFrameworkCore.Web.Mvc.TagHelpers.Ajax
{
    [HtmlTargetElement("a", Attributes = ModalLinkAttributeName)]
    public class ModalLinkTagHelper : TagHelper
    {
        private const string Href = "href";
        private const string ModalLinkAttributeName = "asp-modal-link";
        private const string ModalToggleAttributeName = "asp-modal-toggle";

        [HtmlAttributeName(ModalToggleAttributeName)]
        public bool ModalToggle { get; set; } = true;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var href = output.Attributes[Href].Value;
            output.Attributes.SetAttribute(Href, "#");
            output.Attributes.Add("role", "button");
            output.Attributes.Add("data-ajax", "true");
            output.Attributes.Add("data-ajax-method", "GET");
            output.Attributes.Add("data-ajax-url", href);
            output.Attributes.Add("data-ajax-cache", "false");
            output.Attributes.Add("data-ajax-success", "handleModalLinkLoaded");
            output.Attributes.Add("data-ajax-failure", "handleModalLinkFailed");
            output.Attributes.Add("data-ajax-update", "#main-modal div.modal-content");
            if (ModalToggle)
            {
                output.Attributes.Add("data-toggle", "modal");
                output.Attributes.Add("data-target", "#main-modal");
            }
        }
    }
}