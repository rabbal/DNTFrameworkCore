using DNTFrameworkCore.TestWebApp.Models;
using DNTFrameworkCore.Web.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace DNTFrameworkCore.TestWebApp.Controllers
{
    public class SharedController : Controller
    {
        [HttpGet, AjaxOnly]
        public IActionResult RenderDeleteConfirmation(DeleteConfirmationModel model)
        {
            return PartialView("_DeleteConfirmation", model);
        }
    }
}