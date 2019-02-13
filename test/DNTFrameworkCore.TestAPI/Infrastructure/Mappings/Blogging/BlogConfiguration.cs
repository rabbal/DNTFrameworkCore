using DNTFrameworkCore.TestAPI.Domain.Blogging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNTFrameworkCore.TestAPI.Infrastructure.Mappings.Blogging
{
    public class BlogConfiguration : IEntityTypeConfiguration<Blog>
    {
        public void Configure(EntityTypeBuilder<Blog> builder)
        {
            builder.Property(b => b.Title).IsRequired().HasMaxLength(Blog.MaxTitleLength);
            builder.Property(b => b.NormalizedTitle).IsRequired().HasMaxLength(Blog.MaxTitleLength);
            builder.Property(b => b.Url).IsRequired().HasMaxLength(Blog.MaxUrlLength);

            builder.HasIndex(b => b.NormalizedTitle).HasName("Blog_NormalizedTitle").IsUnique();
        }
    }
}