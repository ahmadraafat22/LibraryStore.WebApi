using FluentValidation;
using LibraryStore.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryStore.Application.Features.Categories.Commands.CreateCategory
{
    public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
    {
        private readonly ILibraryDbContext _context;
        public CreateCategoryCommandValidator(ILibraryDbContext context)
        {
            _context = context;
            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("يجب ادخال اسم القسم")
                .MaximumLength(100).WithMessage("الاسم يجب ألا يتعدي 100 حرف ")
                .MustAsync(async (name, cancellationToken) =>
                !await _context.Categories
                .AnyAsync(c => c.Name == name.Trim(), cancellationToken))
                .WithMessage("هذا الاسم موجود بالفعل ");

            RuleFor(c => c.Description)
                .MaximumLength(400).WithMessage("الوصف لا يتعدي 400 حرف ");
        }
    }
}
