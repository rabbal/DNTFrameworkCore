using System.ComponentModel.DataAnnotations;
using DNTFrameworkCore.Application.Models;
using DNTFrameworkCore.TestWebApp.Domain.Catalog;

namespace DNTFrameworkCore.TestWebApp.Application.Catalog.Models
{
    public class ProductModel : MasterModel<long>
    {
        [Required(ErrorMessage = "Product.Fields.Title.Required")]
        [StringLength(Product.MaxTitleLength, MinimumLength = 3, ErrorMessage = "Product.Fields.Title.StringLength")]
        public string Title { get; set; }
        [StringLength(50, ErrorMessage = "Product.Fields.Number.MaximumLength")]
        [RegularExpression("^[a-zA-Z0-9_]*$", ErrorMessage = "Product.Fields.Number.RegularExpression")]
        public string Number { get; set; }
        [Required(ErrorMessage = "Product.Fields.Price.Required")]
        public decimal Price { get; set; }
    }
}