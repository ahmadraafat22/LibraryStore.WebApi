using LibraryStore.Application.Common.Exceptions;
using LibraryStore.Domain.Entities;
using LibraryStore.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryStore.Application.Features.Categories.Commands.SoftDeleteCategory
{
    public class SoftDeleteCategoryCommandHandler : IRequestHandler<SoftDeleteCategoryCommand>
    {
        private readonly ILibraryDbContext _context;
        public SoftDeleteCategoryCommandHandler(ILibraryDbContext context)
        {
            _context = context;
        }

        public async Task Handle(SoftDeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == request.Id);

            if (category == null)
            {
                throw new NotFoundException(nameof(Category), request.Id);
            }


            category.IsDeleted = true;
            await _context.SaveChangesAsync(cancellationToken);

        }
    }
}
