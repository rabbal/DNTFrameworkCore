using System.Collections.Generic;
using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.TestWebApp.Domain.Invoices
{
    public class Invoice : TrackableEntity<long>, IHasRowVersion, ICreationTracking, IModificationTracking,
        INumberedEntity
    {
        public const int MaxDescriptionLength = 1024;

        public string Number { get; set; }
        public string Description { get; set; }
        public byte[] RowVersion { get; set; }

        public ICollection<InvoiceItem> Items { get; set; } = new HashSet<InvoiceItem>();
    }
}