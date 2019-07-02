using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.TestWebApp.Domain.Invoices
{
    /// <summary>
    /// Represents an invoice status enumeration
    /// </summary>
    public class InvoiceStatus : Enumeration
    {
        public static InvoiceStatus Pending = new InvoiceStatus(1, nameof(Pending));
        public static InvoiceStatus Processing = new InvoiceStatus(2, nameof(Processing));
        public static InvoiceStatus Complete = new InvoiceStatus(3, nameof(Complete));
        public static InvoiceStatus Cancelled = new InvoiceStatus(4, nameof(Cancelled));

        private InvoiceStatus(int value, string name)
            : base(value, name)
        {
        }
    }
}