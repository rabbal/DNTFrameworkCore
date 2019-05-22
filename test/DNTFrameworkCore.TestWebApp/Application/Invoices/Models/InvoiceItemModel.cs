using System.ComponentModel.DataAnnotations;
using DNTFrameworkCore.Application.Models;

namespace DNTFrameworkCore.TestWebApp.Application.Invoices.Models
{
    public class InvoiceItemModel : Model<long>
    {
        [Required(ErrorMessage = "InvoiceItem.Fields.Quantity.Required")]
        public int Quantity { get; set; }
        [Required(ErrorMessage = "InvoiceItem.Fields.UnitPrice.Required")]
        public decimal UnitPrice { get; set; }
        [Required(ErrorMessage = "InvoiceItem.Fields.UnitDiscount.Required")]
        public decimal UnitDiscount { get; set; }
        [Required(ErrorMessage = "InvoiceItem.Fields.ProductId.Required")]
        public long ProductId { get; set; }
    }
}