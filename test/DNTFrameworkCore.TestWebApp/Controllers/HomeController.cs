using System;
using System.Diagnostics;
using System.Threading.Tasks;
using DNTFrameworkCore.TestWebApp.Application.Blogging;
using DNTFrameworkCore.TestWebApp.Application.Blogging.Models;
using Microsoft.AspNetCore.Mvc;
using DNTFrameworkCore.TestWebApp.Models;

namespace DNTFrameworkCore.TestWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBlogService _service;

        public HomeController(IBlogService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async Task<IActionResult> Index()
        {
            var result = await _service.CreateAsync(new BlogModel
                {Title = string.Empty, Url = "blog1.blogging.info"});

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}