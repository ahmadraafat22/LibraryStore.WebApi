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

            // should add product image id validation here 

            RuleFor(pi => pi.ProductImageId)
                .NotEmpty().WithMessage("يجب تحديد الصورة")
                .MustAsync(async (command, productImageId, cancellationToken) =>
                    await _context.ProductImages.AnyAsync(
                        pi => pi.Id == productImageId && pi.ProductId == command.ProductId, cancellationToken)
                ).WithMessage("الصورة غير موجودة او لا تنتمي لهذا المنتج");

            RuleFor(pi => pi.ProductId)
                .NotEmpty().WithMessage("يجب تحديد المنتج")
                .MustAsync(async (productId, cancellationToken) =>
                await _context.Products
                .AnyAsync(p => p.Id == productId, cancellationToken))
                .WithMessage("المنتج غير موجود");

            RuleFor(pi => pi.ImageUrl)
                .NotEmpty().WithMessage("يجب ادخال رابط الصورة")
                .MaximumLength(255).WithMessage("الرابط لا يجب ان يتعدي 255 حرف ");


        }
    }
}
