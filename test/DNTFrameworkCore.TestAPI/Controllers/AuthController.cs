using System;
using System.Net;
using System.Threading.Tasks;
using DNTFrameworkCore.TestAPI.Authentication;
using DNTFrameworkCore.TestAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DNTFrameworkCore.TestAPI.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _service;

        public AuthController(IAuthenticationService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpPost("[action]")]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Token), (int) HttpStatusCode.OK)]
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
        [ProducesResponseType((int) HttpStatusCode.OK)]
        public async Task<IActionResult> Logout()
        {
            await _service.SignOutAsync();

            return Ok();
        }
    }
}