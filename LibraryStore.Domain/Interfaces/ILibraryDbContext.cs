using LibraryStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace LibraryStore.Domain.Interfaces
{
    public interface ILibraryDbContext
    {
        public DbSet<Category> Categories { get; }
        public DbSet<Product> Products { get; }
        public DbSet<ProductImage> ProductImages { get; }
        public DbSet<ProductCategory> ProductCategories { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
