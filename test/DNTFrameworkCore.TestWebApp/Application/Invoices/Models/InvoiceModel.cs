using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DNTFrameworkCore.Application.Models;
using DNTFrameworkCore.TestWebApp.Domain.Invoices;

namespace DNTFrameworkCore.TestWebApp.Application.Invoices.Models
{
    public class InvoiceModel : MasterModel<long>
    {
        [StringLength(50, ErrorMessage = "Invoice.Fields.Number.MaximumLength")]
        [RegularExpression("^[a-zA-Z0-9_]*$", ErrorMessage = "Invoice.Fields.Number.RegularExpression")]
        public string Number { get; set; }
        [StringLength(Invoice.MaxDescriptionLength, ErrorMessage = "Invoice.Fields.Description.MaximumLength")]
        public string Description { get; set; }
        public IList<InvoiceItemModel> Items { get; set; } = new List<InvoiceItemModel>();
    }
}