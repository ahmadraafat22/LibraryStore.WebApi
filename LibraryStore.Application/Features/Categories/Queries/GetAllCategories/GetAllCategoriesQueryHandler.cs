using LibraryStore.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryStore.Application.Features.Categories.Queries.GetAllCategories
{
    public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, ICollection<GetAllCategoryQueryDto>>
    {
        private readonly ILibraryDbContext _context;
        public GetAllCategoriesQueryHandler(ILibraryDbContext context)
        {
            _context = context;
        }
        public async Task<ICollection<GetAllCategoryQueryDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        {

            return await _context.Categories.AsNoTracking()
                .OrderBy(c => c.Id)
                .Select(c =>
                new GetAllCategoryQueryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description
                }).ToListAsync(cancellationToken);
        }
    }
}
