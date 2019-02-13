using System.Xml.Serialization;
using DNTFrameworkCore.GuardToolkit;
using Microsoft.AspNetCore.Mvc;

namespace DNTFrameworkCore.Web.Results
{
    public class XmlResult : ActionResult
    {
        private readonly object _data;

        public XmlResult(object data)
        {
            Guard.ArgumentNotNull(data, nameof(data));

            _data = data;
        }

        public override void ExecuteResult(ActionContext context)
        {
            Guard.ArgumentNotNull(context, nameof(context));

            var response = context.HttpContext.Response;

            var serializer = new XmlSerializer(_data.GetType());

            response.ContentType = "text/xml";
            serializer.Serialize(response.Body, _data);
        }
    }
}