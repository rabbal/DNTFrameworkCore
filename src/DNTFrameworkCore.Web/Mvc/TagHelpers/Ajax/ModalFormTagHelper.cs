using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DNTFrameworkCore.Web.Mvc.TagHelpers.Ajax
{
    [HtmlTargetElement("form", Attributes = ModalFormAttributeName)]
    public class ModalFormTagHelper : TagHelper
    {
        private const string ModalFormAttributeName = "asp-modal-form";

        [HtmlAttributeName(ModalFormAttributeName)]
        public string FormId { get; set; }
        
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.Add("id", FormId);
            output.Attributes.Add("novalidate", "novalidate");
            output.Attributes.Add("autocomplete", "off");
            output.Attributes.Add("data-ajax", "true");
            output.Attributes.Add("data-ajax-method", "POST");
            output.Attributes.Add("data-ajax-begin", "handleModalFormBegin");
            output.Attributes.Add("data-ajax-success", "handleModalFormSucceeded");
            output.Attributes.Add("data-ajax-complete", "handleModalFormComplete");
            output.Attributes.Add("data-ajax-failure", $"handleModalFormFailed(xhr,status,error,'{FormId}')");
            output.Attributes.Add("data-ajax-update", "#main-modal div.modal-content");
        }
    }
}