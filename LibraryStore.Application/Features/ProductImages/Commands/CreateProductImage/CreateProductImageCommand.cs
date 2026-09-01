using MediatR;

namespace LibraryStore.Application.Features.ProductImages.Commands.CreateImage
{
    public class CreateProductImageCommand : IRequest
    {
        public Guid ProductId { get; set; }
        public string ImageUrl { get; set; }
        public bool IsPrimary { get; set; }
        public int DisplayOrder { get; set; }
    }
}
