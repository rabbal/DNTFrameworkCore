using Microsoft.AspNetCore.Mvc;

namespace DNTFrameworkCore.Web.API
{
    //Under development
    public abstract class Endpoint<TRequest, TResponse> : BaseEndpoint
    {
        public abstract ActionResult<TResponse> Handle(TRequest request);
    }

    public abstract class Endpoint<TResponse> : BaseEndpoint
    {
        public abstract ActionResult<TResponse> Handle();
    }

    [ApiController]
    public abstract class BaseEndpoint : ControllerBase
    {
    }
}