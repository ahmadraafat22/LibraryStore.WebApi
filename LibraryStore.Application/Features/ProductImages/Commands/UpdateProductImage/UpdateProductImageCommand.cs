using MediatR;
using Microsoft.AspNetCore.Http;

namespace LibraryStore.Application.Features.ProductImages.Commands.UpdateProductImage
{
    public class UpdateProductImageCommand : IRequest
    {
        public Guid ProductImageId { get; set; }
        public IFormFile? Image { get; set; }
        public bool IsPrimary { get; set; }
    }
}
