using MediatR;

namespace LibraryStore.Application.Features.ProductImages.Commands.SoftDeleteProductImage
{
    public class SoftDeleteProductImageCommand : IRequest
    {
        public Guid ProductImageId { get; set; }

        public SoftDeleteProductImageCommand(Guid id)
        {
            ProductImageId = id;
        }
    }
}
