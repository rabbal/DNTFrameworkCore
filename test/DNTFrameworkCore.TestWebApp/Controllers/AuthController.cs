using System;
using System.Threading.Tasks;
using DNTFrameworkCore.TestWebApp.Authentication;
using DNTFrameworkCore.TestWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DNTFrameworkCore.TestWebApp.Controllers
{
    [AllowAnonymous]
    public class AuthController : Controller
    {
        private readonly IAuthenticationService _service;

        public AuthController(IAuthenticationService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (model == null) return BadRequest("model is not set.");

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _service.SignInAsync(model.UserName, model.Password);

            if (result.Succeeded) return RedirectToAction("Index", "Home");

            ModelState.AddModelError(string.Empty, result.Message);
            return BadRequest(ModelState);
        }

        [HttpPost("[action]"), HttpGet("[action]")]
        public async Task<IActionResult> Logout()
        {
            await _service.SignOutAsync();

            return Ok();
        }
    }
}