using System;
using System.Threading.Tasks;
using DNTFrameworkCore.Web.Mvc.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DNTFrameworkCore.Web.Mvc.TagHelpers
{
    [HtmlTargetElement("delete-confirmation", Attributes = ModelIdAttributeName, TagStructure = TagStructure.WithoutEndTag)]
    public class DeleteConfirmationTagHelper : TagHelper
    {
        private const string ModelIdAttributeName = "asp-model-id";
        private const string ActionAttributeName = "asp-action";
        private const string ControllerAttributeName = "asp-controller";
        private const string AreaAttributeName = "asp-area";

        private readonly IHtmlHelper _htmlHelper;

        [HtmlAttributeName(ModelIdAttributeName)]
        public string ModelId { get; set; }
        [HtmlAttributeName(ActionAttributeName)]
        public string Action { get; set; }
        [HtmlAttributeName(ControllerAttributeName)]
        public string Controller { get; set; }
        [HtmlAttributeName(AreaAttributeName)]
        public string Area { get; set; }

        [HtmlAttributeNotBound, ViewContext]
        public ViewContext ViewContext { get; set; }

        public DeleteConfirmationTagHelper(IHtmlHelper htmlHelper)
        {
            _htmlHelper = htmlHelper ?? throw new ArgumentNullException(nameof(htmlHelper));
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            //contextualize IHtmlHelper
            var viewContextAware = _htmlHelper as IViewContextAware;
            viewContextAware?.Contextualize(ViewContext);

            if (string.IsNullOrEmpty(Action))
                Action = "Delete";

            var model = new DeleteConfirmationModel
            {
                ModelId = ModelId,
                ControllerName = Controller,
                ActionName = Action,
                AreaName = Area
            };

            output.TagName = null;
            output.Content.SetHtmlContent(await _htmlHelper.PartialAsync("_DeleteConfirmation", model));
        }
    }
}