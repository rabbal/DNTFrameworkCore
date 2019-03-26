using System;
using System.Collections.Generic;
using System.Linq;
using DNTFrameworkCore.Runtime;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DNTFrameworkCore.Web.Mvc.TagHelpers
{
    [HtmlTargetElement(Attributes = HasPermissionAttributeName)]
    public class HasPermissionTagHelper : TagHelper
    {
        private const string HasPermissionAttributeName = "has-permission";
        private readonly IUserSession _session;

        [HtmlAttributeName(HasPermissionAttributeName)]
        public string PermissionName { get; set; }

        public HasPermissionTagHelper(IUserSession session)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!_session.IsAuthenticated || !_session.IsGranted(PermissionName))
            {
                output.SuppressOutput();
            }
        }
    }
}