using MediatR;
using Microsoft.AspNetCore.Http;

namespace LibraryStore.Application.Features.ProductImages.Commands.CreateImage
{
    public class CreateProductImageCommand : IRequest<Guid>
    {
        public Guid ProductId { get; set; }
        public IFormFile Image { get; set; }
        public bool IsPrimary { get; set; }
        public int DisplayOrder { get; set; }
    }
}
