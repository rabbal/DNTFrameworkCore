using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectName.API.Authentication;
using ProjectName.API.Models;

namespace ProjectName.API.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [IgnoreAntiforgeryToken]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _service;

        public AuthController(IAuthenticationService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpPost("[action]")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Token), StatusCodes.Status200OK)]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (model == null) return BadRequest("model is not set.");

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _service.SignInAsync(model.UserName, model.Password);

            if (!result.Failed) return Ok(result.Token);

            ModelState.AddModelError(string.Empty, result.Message);
            return BadRequest(ModelState);
        }

        [HttpPost("[action]"), HttpGet("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task Logout()
        {
            await _service.SignOutAsync();
        }
    }
}