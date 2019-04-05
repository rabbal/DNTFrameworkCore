using System;
using DNTFrameworkCore.Application.Models;

namespace DNTFrameworkCore.TestWebApp.Application.Invoices.Models
{
    public class InvoiceReadModel : Model<long>
    {
        public string Number { get; set; }
        public DateTimeOffset CreationDateTime { get; set; }
    }
}