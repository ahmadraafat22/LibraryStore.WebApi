using FluentValidation;
using LibraryStore.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryStore.Application.Features.ProductImages.Commands.UpdateProductImage
{
    public class UpdateProductImageValidator : AbstractValidator<UpdateProductImageCommand>
    {
        private readonly ILibraryDbContext _context;

        public UpdateProductImageValidator(ILibraryDbContext context)
        {
            _context = context;

            RuleFor(pi => pi.ProductImageId)
                .NotEmpty().WithMessage("يجب تحديد الصورة")
                .MustAsync(async (productImageId, cancellationToken) =>
                    await _context.ProductImages
                        .AnyAsync(pi => pi.Id == productImageId, cancellationToken))
                .WithMessage("الصورة غير موجودة");


            When(pi => pi.Image is not null, () =>
            {
                RuleFor(pi => pi.Image!)
                    .Must(image => image.Length > 0)
                    .WithMessage("الصورة غير صالحة")
                    .Must(image =>
                        image.ContentType == "image/jpeg" ||
                        image.ContentType == "image/png" ||
                        image.ContentType == "image/webp")
                    .WithMessage("يسمح فقط بصور JPG أو PNG أو WEBP")
                    .Must(image => image.Length <= 5 * 1024 * 1024)
                    .WithMessage("حجم الصورة يجب ألا يتجاوز 5 MB");
            });
        }
    }
}