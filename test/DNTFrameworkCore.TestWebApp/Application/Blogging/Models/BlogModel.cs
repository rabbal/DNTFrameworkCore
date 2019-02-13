using System.ComponentModel.DataAnnotations;
using DNTFrameworkCore.Application.Models;

namespace DNTFrameworkCore.TestWebApp.Application.Blogging.Models
{
    public class BlogModel : MasterModel<int>
    {
        public string Title { get; set; }

        [MinLength(1000, ErrorMessage = "MinLength 1000")]
        public string Url { get; set; }
    }
}