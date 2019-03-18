using System.ComponentModel.DataAnnotations;
using DNTFrameworkCore.Application.Models;

namespace DNTFrameworkCore.TestWebApp.Application.Blogging.Models
{
    public class BlogModel : MasterModel<int>
    {
        public string Title { get; set; }
        public string Url { get; set; }
    }
}