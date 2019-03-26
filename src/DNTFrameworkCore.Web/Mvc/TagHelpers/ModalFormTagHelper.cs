using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DNTFrameworkCore.Web.Mvc.TagHelpers
{
    [HtmlTargetElement("form", Attributes = ModalFormAttributeName)]
    public class ModalFormTagHelper : TagHelper
    {
        private const string ModalFormAttributeName = "modal-form";

        [HtmlAttributeName(ModalFormAttributeName)]
        public string Id { get; set; }
        public ModalFormTagHelper() : base()
        {
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.Add("id", Id);
            output.Attributes.Add("novalidate", "novalidate");
            output.Attributes.Add("autocomplete", "off");
            output.Attributes.Add("data-ajax", "true");
            output.Attributes.Add("data-ajax-method", "POST");
            output.Attributes.Add("data-ajax-begin", "handleModalFormBegin");
            output.Attributes.Add("data-ajax-success", "handleModalFormLoaded");
            output.Attributes.Add("data-ajax-complete", "handleModalFormComplete");
            output.Attributes.Add("data-ajax-failure", $"handleModalFormFailed(xhr,status,error,'{Id}')");
            output.Attributes.Add("data-ajax-update", "#main-modal div.modal-content");
        }
    }
}