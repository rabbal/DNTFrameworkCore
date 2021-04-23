using System.Threading.Tasks;
using DNTFrameworkCore.TestCqrsAPI.Authorization;
using DNTFrameworkCore.TestCqrsAPI.Domain.Catalog.Commands;
using DNTFrameworkCore.Web.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DNTFrameworkCore.TestCqrsAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    public class PriceTypesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PriceTypesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        //[HttpPost, PermissionAuthorize(Permissions.PriceTypes_Create)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePriceTypeCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.Failed) return BadRequest(result.Message);

            return Ok();
        }

        [HttpPost, PermissionAuthorize(Permissions.PriceTypes_Remove)]
        public async Task<IActionResult> Remove([FromBody] RemovePriceTypeCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.Failed) return BadRequest(result.Message);

            return Ok();
        }
    }
}