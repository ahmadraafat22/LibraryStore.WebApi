using LibraryStore.Application.Common.Exceptions;
using LibraryStore.Application.Common.Interfaces;
using LibraryStore.Domain.Entities;
using LibraryStore.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryStore.Application.Features.ProductImages.Commands.UpdateProductImage
{
    public class UpdateProductImageCommandHandler
        : IRequestHandler<UpdateProductImageCommand>
    {
        private readonly ILibraryDbContext _context;
        private readonly ICloudinaryService _cloudinaryService;

        public UpdateProductImageCommandHandler(ILibraryDbContext context, ICloudinaryService cloudinaryService)
        {
            _context = context;
            _cloudinaryService = cloudinaryService;
        }

        public async Task Handle(UpdateProductImageCommand request, CancellationToken cancellationToken)
        {
            var image = await _context.ProductImages
                .FirstOrDefaultAsync(
                    pi => pi.Id == request.ProductImageId,
                    cancellationToken);

            if (image == null)
            {
                throw new NotFoundException(
                    nameof(ProductImage),
                    request.ProductImageId);
            }

            bool wasPrimary = image.IsPrimary;

            if (request.Image != null)
            {
                var oldPublicId = image.PublicId;

                var uploadResult = await _cloudinaryService.UploadImageAsync(
                    request.Image,
                    cancellationToken);

                image.ImageUrl = uploadResult.Url;
                image.PublicId = uploadResult.PublicId;


                await _cloudinaryService.DeleteImageAsync(
                    oldPublicId,
                    cancellationToken);
            }

            if (request.IsPrimary)
            {
                var currentPrimaryImages = await _context.ProductImages
                    .Where(pi =>
                        pi.ProductId == image.ProductId &&
                        pi.Id != image.Id &&
                        pi.IsPrimary)
                    .ToListAsync(cancellationToken);

                foreach (var img in currentPrimaryImages)
                {
                    img.IsPrimary = false;
                }

                var otherImages = await _context.ProductImages
                    .Where(pi =>
                        pi.ProductId == image.ProductId &&
                        pi.Id != image.Id)
                    .OrderBy(pi => pi.DisplayOrder)
                    .ToListAsync(cancellationToken);

                int order = 1;

                foreach (var img in otherImages)
                {
                    img.DisplayOrder = order++;
                }

                image.IsPrimary = true;
                image.DisplayOrder = 0;
            }

            else if (wasPrimary)
            {
                var newPrimaryImage = await _context.ProductImages
                    .Where(pi =>
                        pi.ProductId == image.ProductId &&
                        pi.Id != image.Id)
                    .OrderBy(pi => pi.DisplayOrder)
                    .FirstOrDefaultAsync(cancellationToken);

                if (newPrimaryImage == null)
                {
                    throw new InvalidOperationException(
                        "لا يمكن إزالة الصورة الرئيسية، يجب وجود صورة أخرى.");
                }

                newPrimaryImage.IsPrimary = true;
                newPrimaryImage.DisplayOrder = 0;

                var otherImages = await _context.ProductImages
                    .Where(pi =>
                        pi.ProductId == image.ProductId &&
                        pi.Id != image.Id &&
                        pi.Id != newPrimaryImage.Id)
                    .OrderBy(pi => pi.DisplayOrder)
                    .ToListAsync(cancellationToken);

                int order = 1;

                foreach (var img in otherImages)
                {
                    img.DisplayOrder = order++;
                }

                image.IsPrimary = false;
            }
            else
            {
                image.IsPrimary = false;
            }

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}