using FluentValidation;
using LibraryStore.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryStore.Application.Features.ProductImages.Commands.CreateImage
{
    public class CreateProductImageValidator : AbstractValidator<CreateProductImageCommand>
    {
        private readonly ILibraryDbContext _context;
        public CreateProductImageValidator(ILibraryDbContext context)
        {
            _context = context;


            RuleFor(pi => pi.ProductId)
                .NotEmpty().WithMessage("يجب تحديد المنتج")
                .MustAsync(async (productId, cancellationToken) =>
                await _context.Products
                .AnyAsync(p => p.Id == productId, cancellationToken))
                .WithMessage("المنتج غير موجود");

            RuleFor(pi => pi.ImageUrl)
                .NotEmpty().WithMessage("يجب ادخال رابط الصورة")
                .MaximumLength(255).WithMessage("الرابط لا يجب ان يتعدي 255 حرف ");

            RuleFor(pi => pi.DisplayOrder)
                .GreaterThanOrEqualTo(0).WithMessage("يجب ان يكون الترتيب اكبر من او يساور 0");

            RuleFor(pi => pi.ProductId)
                .MustAsync(async (productId, cancellationToken) =>
                    await _context.ProductImages
                    .CountAsync(pi => pi.ProductId == productId, cancellationToken) < 3
                    ).WithMessage("لا يمكن اضافة اكثر من 3 صور للمنتج");

        }
    }
}
