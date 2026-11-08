using LibraryStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryStore.Infrastructure.Persistence.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.HasKey(oi => oi.Id);

            builder.Property(oi => oi.ProductName)
                .HasMaxLength(200)
                .IsRequired();
            builder.Property(oi => oi.Quantity)
                .IsRequired();
            builder.Property(oi => oi.UnitPrice)
                .HasPrecision(18, 2)
                .IsRequired();
            builder.Property(oi => oi.LineTotal)
                .HasPrecision(18, 2)
                .IsRequired();
            builder.HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId);
            builder.ToTable("OrderItems", table =>
            {
                table.HasCheckConstraint("CK_OrderItem_Quantity_Positive", "[Quantity]>0");
            });
        }
    }
}
