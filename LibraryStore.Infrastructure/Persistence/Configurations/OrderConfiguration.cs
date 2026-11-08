using LibraryStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryStore.Infrastructure.Persistence.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);
            builder.Property(o => o.CustomerName)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(o => o.CustomerAddress)
                .HasMaxLength(400)
                .IsRequired();

            builder.Property(o => o.CustomerPhone)
                .HasMaxLength(11)
                .IsRequired();

            builder.Property(o => o.Total)
                .HasPrecision(18, 2);

            builder.Property(o => o.OrderNumber)
                .HasMaxLength(20)
                .IsRequired();

            builder.HasIndex(o => o.OrderNumber)
                .IsUnique();

            builder.Property(o => o.Status)
                .HasConversion<string>()
                .IsRequired();

            builder.HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId);

            builder.HasQueryFilter(o => !o.IsDeleted);
        }
    }
}
