using LibraryStore.Application.Common.Exceptions;
using LibraryStore.Domain.Entities;
using LibraryStore.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryStore.Application.Features.ProductImages.Commands.UpdateProductImage
{
    public class UpdateProductImageCommandHandler : IRequestHandler<UpdateProductImageCommand>
    {
        private readonly ILibraryDbContext _context;
        public UpdateProductImageCommandHandler(ILibraryDbContext context)
        {
            _context = context;
        }
        public async Task Handle(UpdateProductImageCommand request, CancellationToken cancellationToken)
        {
            var image = await _context.ProductImages
                .FirstOrDefaultAsync(pi => pi.Id == request.ProductImageId, cancellationToken);

            if (image == null)
            {
                throw new NotFoundException(nameof(ProductImage), request.ProductImageId);
            }
            if (request.IsPrimary)
            {
                var currentPrimaryImage = await _context.ProductImages
                    .Where(pi => pi.ProductId == request.ProductId && pi.IsPrimary)
                    .ToListAsync(cancellationToken);
                foreach (var img in currentPrimaryImage)
                {
                    img.IsPrimary = false;
                }
            }
            bool wasPrimary = image.IsPrimary;
            image.ImageUrl = request.ImageUrl;
            image.IsPrimary = request.IsPrimary;

            if (request.IsPrimary)
            {
                var images = await _context.ProductImages
                    .Where(pi => pi.ProductId == request.ProductId && pi.Id != request.ProductImageId)
                    .OrderBy(pi => pi.DisplayOrder)
                    .ToListAsync(cancellationToken);
                int order = 1;
                foreach (var img in images)
                {
                    img.DisplayOrder = order;
                    order++;
                }
                image.DisplayOrder = 0;
            }
            else if (wasPrimary)
            {
                var newPrimaryImage = await _context.ProductImages
                    .Where(pi => pi.ProductId == request.ProductId && pi.Id != request.ProductImageId)
                    .OrderBy(pi => pi.DisplayOrder)
                    .FirstOrDefaultAsync(cancellationToken);
                if (newPrimaryImage == null)
                {
                    throw new Exception("لا يوجد صور اخرى لتكون الرئيسية");
                }

                var images = await _context.ProductImages
                    .Where(pi => pi.ProductId == request.ProductId && pi.Id != newPrimaryImage.Id)
                    .OrderBy(pi => pi.DisplayOrder)
                    .ToListAsync(cancellationToken);
                newPrimaryImage.DisplayOrder = 0;
                newPrimaryImage.IsPrimary = true;
                int order = 1;
                foreach (var img in images)
                {
                    img.DisplayOrder = order;
                    order++;
                }
            }
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
