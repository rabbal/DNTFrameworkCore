using Microsoft.AspNetCore.Mvc;

namespace DNTFrameworkCore.Web.Results
{
    public class JavaScriptResult : ContentResult
    {
        public string Script { get => Content; set => Content = value; }
        
        public JavaScriptResult()
        {
            ContentType = "application/x-javascript";
        }
    }
}