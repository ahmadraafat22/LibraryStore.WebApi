using FluentValidation;
using LibraryStore.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryStore.Application.Features.Categories.Commands.UpdateCategory
{
    public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryCommand>
    {
        private readonly ILibraryDbContext _context;
        public UpdateCategoryValidator(ILibraryDbContext context)
        {
            _context = context;
            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("يجب ادخال اسم القسم")
                .MaximumLength(100).WithMessage("الاسم يجب ألا يتعدي 100 حرف ")
                .MustAsync(async (command, name, cancellationToken) =>
                !await _context.Categories
                .AnyAsync(c => c.Name == name && c.Id != command.Id && !c.IsDeleted, cancellationToken))
                .WithMessage("هذا الاسم موجود بالفعل ");

            RuleFor(c => c.Description)
                .MaximumLength(400).WithMessage("الوصف لا يتعدي 400 حرف ");
        }
    }
}
