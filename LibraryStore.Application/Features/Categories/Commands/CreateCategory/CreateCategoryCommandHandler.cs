using LibraryStore.Domain.Entities;
using LibraryStore.Domain.Interfaces;
using MediatR;

namespace LibraryStore.Application.Features.Categories.Commands.CreateCategory
{
    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Guid>
    {
        private readonly ILibraryDbContext _context;
        public CreateCategoryCommandHandler(ILibraryDbContext context)
        {
            _context = context;
        }
        public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = new Category()
            {
                Name = request.Name,
                Description = request.Description
            };

            // adding category 
            await _context.Categories.AddAsync(category);

            // save 
            await _context.SaveChangesAsync(cancellationToken);

            return category.Id;
        }
    }
}
