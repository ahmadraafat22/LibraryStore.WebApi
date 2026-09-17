using LibraryStore.Application.Common.Exceptions;
using LibraryStore.Domain.Entities;
using LibraryStore.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryStore.Application.Features.ProductImages.Commands.SoftDeleteProductImage
{
    public class SoftDeleteProductImageCommandHandler : IRequestHandler<SoftDeleteProductImageCommand>
    {
        private readonly ILibraryDbContext _context;
        public SoftDeleteProductImageCommandHandler(ILibraryDbContext context)
        {
            _context = context;
        }

        public async Task Handle(SoftDeleteProductImageCommand request, CancellationToken cancellationToken)
        {
            var image = await _context.ProductImages
                .FirstOrDefaultAsync(pi => pi.Id == request.ProductImageId, cancellationToken);
            if (image == null)
            {
                throw new NotFoundException(nameof(ProductImage), request.ProductImageId);
            }
            var count = await _context.ProductImages
                .Where(pi => pi.ProductId == image.ProductId)
                .CountAsync(cancellationToken);
            if (image.IsPrimary && count > 1)
            {
                var newPrimaryImage = await _context.ProductImages
                    .Where(pi => pi.ProductId == image.ProductId && pi.Id != image.Id)
                    .OrderBy(pi => pi.DisplayOrder)
                    .FirstOrDefaultAsync(pi => pi.ProductId == image.ProductId && pi.Id != image.Id, cancellationToken);
                newPrimaryImage.IsPrimary = true;
                newPrimaryImage.DisplayOrder = 0;
                var images = await _context.ProductImages
                    .Where(pi => pi.ProductId == image.ProductId && pi.Id != image.Id && pi.Id != newPrimaryImage.Id)
                    .OrderBy(pi => pi.DisplayOrder)
                    .ToListAsync(cancellationToken);
                int order = 1;
                foreach (var img in images)
                {
                    img.DisplayOrder = order;
                    order++;
                }
            }
            image.IsDeleted = true;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
