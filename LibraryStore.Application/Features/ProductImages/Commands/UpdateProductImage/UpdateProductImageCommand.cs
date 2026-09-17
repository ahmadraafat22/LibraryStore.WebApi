using MediatR;

namespace LibraryStore.Application.Features.ProductImages.Commands.UpdateProductImage
{
    public class UpdateProductImageCommand : IRequest
    {
        public Guid ProductId { get; set; }
        public Guid ProductImageId { get; set; }
        public string ImageUrl { get; set; }
        public bool IsPrimary { get; set; }
    }
}
