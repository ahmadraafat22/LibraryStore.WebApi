using LibraryStore.Domain.Entities;
using LibraryStore.Domain.Interfaces;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LibraryStore.Infrastructure.Persistence
{
    public class LibraryDbContext : IdentityDbContext<ApplicationUser>, ILibraryDbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(u => u.NormalizedEmail)
                .IsUnique()
                .HasFilter("[NormalizedEmail] IS NOT NULL");

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(LibraryDbContext).Assembly);
            modelBuilder.HasSequence<int>("OrderSequence")
                .StartsAt(1)
                .IncrementsBy(1);
        }
        public void ApplyAudit()
        {
            var entries = ChangeTracker.Entries<BaseEntity>();
            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                }
            }
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAudit();
            return base.SaveChangesAsync(cancellationToken);
        }
        public override int SaveChanges()
        {
            ApplyAudit();
            return base.SaveChanges();
        }
    }
}
