using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DNTFrameworkCore.TestWebApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace DNTFrameworkCore.TestWebApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {

        public HomeController()
        {
        }

        public IActionResult Index()
        {
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