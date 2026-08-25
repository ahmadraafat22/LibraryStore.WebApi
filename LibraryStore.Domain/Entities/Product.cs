namespace LibraryStore.Domain.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal? OldPrice { get; set; }
        public bool IsAvailable { get; set; } = true;

        public ICollection<ProductImage> Images { get; set; }
            = new List<ProductImage>();

        public ICollection<ProductCategory> ProductCategories { get; set; }
            = new List<ProductCategory>();
    }
}
