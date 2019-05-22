using DNTFrameworkCore.TestWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DNTFrameworkCore.TestWebApp.Helpers
{
    [HtmlTargetElement("a", Attributes = ModalDeleteLinkAttributeName + "," + ModelIdAttributeName)]
    public class ModalDeleteLinkTagHelper : TagHelper
    {
        private const string ModelIdAttributeName = "asp-model-id";
        private const string ModalDeleteLinkAttributeName = "asp-modal-delete-link";
        private readonly IUrlHelper _urlHelper;

        public ModalDeleteLinkTagHelper(IUrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }

        [HtmlAttributeName(ModelIdAttributeName)]
        public string ModelId { get; set; }
        [HtmlAttributeName("asp-modal-toggle")]
        public bool ModalToggle { get; set; } = true;
        [HtmlAttributeName("asp-title")]
        public string Title { get; set; }
        [HtmlAttributeName("asp-body")]
        public string Body { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var href = output.Attributes["href"].Value;
            var query = new DeleteConfirmationModel
            {
                ModelId = ModelId,
                Url = href.ToString(),
                Title = string.IsNullOrEmpty(Title) ? "Are you sure?" : Title,
                Body = string.IsNullOrEmpty(Body) ? "Are you sure you want to delete this item?" : Body
            };
            output.Attributes.SetAttribute("href", "#");
            output.Attributes.Add("role", "button");
            output.Attributes.Add("data-ajax", "true");
            output.Attributes.Add("data-ajax-method", "GET");
            output.Attributes.Add("data-ajax-url", _urlHelper.Action("RenderDeleteConfirmation", "Shared", query));
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