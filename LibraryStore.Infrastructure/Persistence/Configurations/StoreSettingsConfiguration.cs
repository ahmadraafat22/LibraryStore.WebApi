using LibraryStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryStore.Infrastructure.Persistence.Configurations
{
    public class StoreSettingsConfiguration : IEntityTypeConfiguration<StoreSettings>
    {
        public void Configure(EntityTypeBuilder<StoreSettings> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.StoreName)
                .HasMaxLength(100).IsRequired();

            builder.Property(s => s.Phone)
                .HasMaxLength(20).IsRequired();

            builder.Property(s => s.Address)
                .HasMaxLength(200).IsRequired();

            builder.Property(s => s.Facebook).HasMaxLength(500);
            builder.Property(s => s.Instagram).HasMaxLength(500);
            builder.Property(s => s.WhatsApp).HasMaxLength(500);
            builder.Property(s => s.TikTok).HasMaxLength(500);

            builder.HasData(new StoreSettings
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                StoreName = "LibraryStore",
                Phone = "01000000000",
                Address = "Minya",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });
        }
    }
}
