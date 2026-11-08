using FluentValidation;
using LibraryStore.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryStore.Application.Features.Products.Commands.UpdateProduct
{
    public class UpdateProductValidator : AbstractValidator<UpdateProductCommand>
    {
        private readonly ILibraryDbContext _context;
        public UpdateProductValidator(ILibraryDbContext context)
        {
            _context = context;
            RuleFor(p => p.Name)
                 .NotEmpty()
                 .WithMessage("لا يمكن ترك الاسم فارغ")
                 .MaximumLength(100)
                 .WithMessage("يجب ألا يتجاوز الاسم 100 حرف");

            RuleFor(p => p.Description)
                .MaximumLength(400)
                .WithMessage("يجب ألا يتجاوز الوصف 400 حرف");

            RuleFor(p => p.Price)
                .GreaterThan(0)
                .WithMessage("يجب أن يكون السعر أكبر من صفر")
                .PrecisionScale(18, 2, true);

            RuleFor(p => p.OldPrice)
                .GreaterThan(0)
                .WithMessage("السعر القديم يجب أن يكون أكبر من صفر")
                .PrecisionScale(18, 2, true);

            RuleFor(p => p.OldPrice)
                .GreaterThan(p => p.Price)
                .When(p => p.OldPrice.HasValue)
                .WithMessage("السعر القديم يجب أن يكون أكبر من السعر الحالي");

            RuleForEach(p => p.CategoryIds)
                .MustAsync(async (categoryId, cancellationToken) =>
                    await _context.Categories.AnyAsync(c => c.Id == categoryId, cancellationToken))
                .WithMessage("هذا القسم غير موجود");

            RuleFor(p => p.CategoryIds)
                .Must(ids => ids.Distinct().Count() == ids.Count)
                .WithMessage("لا يمكن تكرار الاقسام");
        }
    }
}
