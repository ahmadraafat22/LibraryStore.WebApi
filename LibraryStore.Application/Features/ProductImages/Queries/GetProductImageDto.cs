namespace LibraryStore.Application.Features.ProductImages.Queries
{
    public class GetProductImageDto
    {
        public Guid Id { get; set; }
        public string ImageUrl { get; set; } = null!;
        public bool IsPrimary { get; set; }
        public int DisplayOrder { get; set; }
    }

}
