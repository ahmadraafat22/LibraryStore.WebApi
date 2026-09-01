using LibraryStore.Application.Features.ProductImages.Queries;
using LibraryStore.Application.Features.Products.Queries.GetProductById;

namespace LibraryStore.Application.Features.Products.Queries.GetAllProducts
{
    public class GetProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal? OldPrice { get; set; }
        public decimal? DiscountPercentage
        {
            get
            {
                if (!OldPrice.HasValue || OldPrice <= 0)
                    return null;

                return ((OldPrice.Value - Price) / OldPrice.Value) * 100;
            }
        }
        public List<GetCategoryDto> Categories { get; set; } = new();
        public List<GetProductImageDto> Images { get; set; }
    }
}
