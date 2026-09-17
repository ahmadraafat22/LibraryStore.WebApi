using LibraryStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryStore.Infrastructure.Persistence.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Name)
                .HasMaxLength(100)
                .IsRequired();
            builder.HasIndex(c => c.Name)
                .IsUnique()
                .HasFilter("[IsDeleted]=0");

            builder.Property(c => c.Description)
                .HasMaxLength(400);
            builder.HasQueryFilter(c => !c.IsDeleted);
        }
    }
}
