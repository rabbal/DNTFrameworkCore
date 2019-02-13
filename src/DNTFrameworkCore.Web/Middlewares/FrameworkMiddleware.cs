using System;
using System.Threading.Tasks;
using DNTFrameworkCore.Infrastructure;
using Microsoft.AspNetCore.Http;

namespace DNTFrameworkCore.Web.Middlewares
{
    public class FrameworkMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _provider;

        public FrameworkMiddleware(RequestDelegate next, IServiceProvider provider)
        {
            _next = next;
            _provider = provider;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await Bootstrapper.RunOnBeginRequest(_provider);

                await _next(context);

                await Bootstrapper.RunOnEndRequest(_provider);
            }
            catch (Exception e)
            {
                await Bootstrapper.RunOnError(_provider, e);
                
                throw;
            }
        }
    }
}