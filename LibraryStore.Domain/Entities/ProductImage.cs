namespace LibraryStore.Domain.Entities
{
    public class ProductImage : BaseEntity
    {
        public Guid ProductId { get; set; }
        public string ImageUrl { get; set; } = null!;
        public bool IsPrimary { get; set; } = false;
        public int DisplayOrder { get; set; }
        public string PublicId { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
