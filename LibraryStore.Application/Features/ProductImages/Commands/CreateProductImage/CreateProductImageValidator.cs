using FluentValidation;
using LibraryStore.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryStore.Application.Features.ProductImages.Commands.CreateImage
{
    public class CreateProductImageValidator
        : AbstractValidator<CreateProductImageCommand>
    {
        private readonly ILibraryDbContext _context;

        public CreateProductImageValidator(ILibraryDbContext context)
        {
            _context = context;

            RuleFor(pi => pi.ProductId)
                .NotEmpty()
                .WithMessage("يجب تحديد المنتج")
                .MustAsync(async (productId, cancellationToken) =>
                    await _context.Products
                        .AnyAsync(p => p.Id == productId, cancellationToken))
                .WithMessage("المنتج غير موجود");

            RuleFor(pi => pi.Image)
                .NotNull()
                .WithMessage("يجب اختيار صورة")
                .Must(image =>
                    image != null &&
                    image.Length > 0)
                .WithMessage("الصورة غير صالحة")
                .Must(image =>
                    image != null &&
                    (image.ContentType == "image/jpeg" ||
                     image.ContentType == "image/png" ||
                     image.ContentType == "image/webp"))
                .WithMessage("يسمح فقط بصور JPG أو PNG أو WEBP")
                .Must(image =>
                    image != null &&
                    image.Length <= 5 * 1024 * 1024)
                .WithMessage("حجم الصورة يجب ألا يتجاوز 5 MB");

            RuleFor(pi => pi.ProductId)
                .MustAsync(async (productId, cancellationToken) =>
                    await _context.ProductImages
                        .CountAsync(
                            pi => pi.ProductId == productId,
                            cancellationToken) < 3)
                .WithMessage("لا يمكن إضافة أكثر من 3 صور للمنتج");
        }
    }
}