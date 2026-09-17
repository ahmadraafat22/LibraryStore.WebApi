using LibraryStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
namespace LibraryStore.Domain.Interfaces
{
    public interface ILibraryDbContext
    {
        public DatabaseFacade Database { get; }
        public DbSet<Category> Categories { get; }
        public DbSet<Product> Products { get; }
        public DbSet<ProductImage> ProductImages { get; }
        public DbSet<ProductCategory> ProductCategories { get; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
