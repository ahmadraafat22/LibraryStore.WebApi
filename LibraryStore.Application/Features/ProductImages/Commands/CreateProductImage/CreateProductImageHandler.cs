using LibraryStore.Domain.Entities;
using LibraryStore.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryStore.Application.Features.ProductImages.Commands.CreateImage
{
    public class CreateProductImageHandler : IRequestHandler<CreateProductImageCommand>
    {
        private readonly ILibraryDbContext _context;
        public CreateProductImageHandler(ILibraryDbContext context)
        {
            _context = context;
        }
        public async Task Handle(CreateProductImageCommand request, CancellationToken cancellationToken)
        {
            var hasPrimaryImage = await _context.ProductImages
                .AnyAsync(pi => pi.ProductId == request.ProductId && pi.IsPrimary, cancellationToken);

            if (request.IsPrimary)
            {
                var currentPrimaryImages = await _context.ProductImages
                    .Where(pi => pi.ProductId == request.ProductId && pi.IsPrimary)
                    .ToListAsync(cancellationToken);
                foreach (var image in currentPrimaryImages)
                {
                    image.IsPrimary = false;
                }
            }
            else if (!hasPrimaryImage)
            {
                request.IsPrimary = true;
            }

            var productImage = new ProductImage
            {
                ProductId = request.ProductId,
                ImageUrl = request.ImageUrl,
                IsPrimary = request.IsPrimary,
                DisplayOrder = request.DisplayOrder
            };
            await _context.ProductImages.AddAsync(productImage, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

        }
    }
}
