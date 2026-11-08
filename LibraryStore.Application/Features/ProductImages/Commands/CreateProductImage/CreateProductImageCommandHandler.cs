using LibraryStore.Application.Common.Interfaces;
using LibraryStore.Domain.Entities;
using LibraryStore.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryStore.Application.Features.ProductImages.Commands.CreateImage
{
    public class CreateProductImageCommandHandler : IRequestHandler<CreateProductImageCommand, Guid>
    {
        private readonly ILibraryDbContext _context;
        private readonly ICloudinaryService _cloudinaryService;
        public CreateProductImageCommandHandler(ILibraryDbContext context, ICloudinaryService cloudinaryService)
        {
            _context = context;
            _cloudinaryService = cloudinaryService;
        }
        public async Task<Guid> Handle(CreateProductImageCommand request, CancellationToken cancellationToken)
        {
            // this to check if the current product has primary image or not 
            var hasPrimaryImage = await _context.ProductImages
                .AnyAsync(pi => pi.ProductId == request.ProductId && pi.IsPrimary, cancellationToken);
            // if the current has not primary image the first image will be the primary by default 
            bool isPrimary = request.IsPrimary || !hasPrimaryImage;

            if (isPrimary)
            {
                // return all primary images to cancel them and make the new one is primary 
                var currentPrimaryImages = await _context.ProductImages
                    .Where(pi => pi.ProductId == request.ProductId && pi.IsPrimary)
                    .ToListAsync(cancellationToken);

                foreach (var image in currentPrimaryImages)
                {
                    image.IsPrimary = false;
                }
            }

            var maxOrder = await _context.ProductImages
                .Where(pi => pi.ProductId == request.ProductId)
                .Select(pi => (int?)pi.DisplayOrder)
                .MaxAsync(cancellationToken);

            var uploadResult = await _cloudinaryService.UploadImageAsync(request.Image, cancellationToken);

            var productImage = new ProductImage
            {
                ProductId = request.ProductId,
                PublicId = uploadResult.PublicId,
                ImageUrl = uploadResult.Url,
                IsPrimary = isPrimary,
                DisplayOrder = isPrimary ? 0 : (maxOrder ?? -1) + 1
            };
            if (productImage.IsPrimary)
            {
                var images = await _context.ProductImages
                    .Where(pi => pi.ProductId == request.ProductId)
                    .ToListAsync(cancellationToken);
                foreach (var img in images)
                {
                    img.DisplayOrder += 1;
                }
            }
            await _context.ProductImages.AddAsync(productImage, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return productImage.Id;
        }
    }
}
