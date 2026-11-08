using LibraryStore.Application.Common.Exceptions;
using LibraryStore.Domain.Entities;
using LibraryStore.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryStore.Application.Features.Categories.Queries.GetCategoryById
{
    public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, GetCategoryByIdDto>
    {
        private readonly ILibraryDbContext _context;
        public GetCategoryByIdQueryHandler(ILibraryDbContext context)
        {
            _context = context;
        }

        public async Task<GetCategoryByIdDto> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var category = await _context.Categories
                .Where(c => c.Id == request.Id)
                .Select(c => new GetCategoryByIdDto
                {
                    Name = c.Name,
                    Description = c.Description
                }
                ).FirstOrDefaultAsync(cancellationToken);

            if (category == null)
            {
                throw new NotFoundException(nameof(Category), request.Id);
            }

            return category;

        }
    }
}
