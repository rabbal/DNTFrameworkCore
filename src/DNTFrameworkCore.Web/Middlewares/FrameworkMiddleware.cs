using System;
using System.Threading.Tasks;
using DNTFrameworkCore.Tasks;
using Microsoft.AspNetCore.Http;

namespace DNTFrameworkCore.Web.Middlewares
{
    //Under Development
    public class FrameworkMiddleware
    {
        private readonly RequestDelegate _next;

        public FrameworkMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ITaskEngine engine)
        {
            try
            {
                await engine.RunOnBeginRequest(context.RequestAborted);

                await _next(context);

                await engine.RunOnEndRequest(context.RequestAborted);
            }
            catch (Exception e)
            {
                await engine.RunOnException(e, context.RequestAborted);

                throw;
            }
        }
    }
}