using LibraryStore.Application.Common.Extentions;
using LibraryStore.Application.Common.Responses;
using LibraryStore.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryStore.Application.Features.Categories.Queries.GetAllCategories
{
    public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, PaginatedResponse<GetAllCategoryQueryDto>>
    {
        private readonly ILibraryDbContext _context;
        public GetAllCategoriesQueryHandler(ILibraryDbContext context)
        {
            _context = context;
        }
        public async Task<PaginatedResponse<GetAllCategoryQueryDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        {

            var query = _context.Categories.AsNoTracking()
            .AsQueryable();
            if (!string.IsNullOrEmpty(request.Search))
            {
                var search = request.Search.Trim();
                query = query.Where(c => EF.Functions.Like(c.Name, $"%{search}%"));
            }
            return await query
                .OrderBy(c => c.Id)
                .Select(c =>
                new GetAllCategoryQueryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description
                }).ToPaginatedResponse(request.PageNumber, request.PageSize, cancellationToken);
        }
    }
}
