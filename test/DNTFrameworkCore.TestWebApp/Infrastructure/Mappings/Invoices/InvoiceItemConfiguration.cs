using DNTFrameworkCore.TestWebApp.Domain.Invoices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNTFrameworkCore.TestWebApp.Infrastructure.Mappings.Invoices
{
    public class InvoiceItemConfiguration : IEntityTypeConfiguration<InvoiceItem>
    {
        public void Configure(EntityTypeBuilder<InvoiceItem> builder)
        {
            builder.HasOne(ii => ii.Invoice).WithMany(i => i.Items).HasForeignKey(ii => ii.InvoiceId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}