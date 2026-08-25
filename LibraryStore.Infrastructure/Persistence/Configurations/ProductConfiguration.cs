using LibraryStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryStore.Infrastructure.Persistence.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Name)
                .HasMaxLength(100)
                .IsRequired();
            builder.Property(p => p.Description)
                .HasMaxLength(400);
            builder.Property(p => p.Price)
                .HasPrecision(18, 2)
                .IsRequired();
            builder.Property(p => p.OldPrice)
                .HasPrecision(18, 2);
            builder.HasMany(p => p.Images)
                .WithOne(i => i.Product)
                .HasForeignKey(p => p.ProductId);
        }
    }
}
