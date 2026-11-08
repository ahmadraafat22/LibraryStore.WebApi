using LibraryStore.Application.Common.Extentions;
using LibraryStore.Application.Common.Responses;
using LibraryStore.Application.Features.ProductImages.Queries;
using LibraryStore.Application.Features.Products.Queries.GetProductById;
using LibraryStore.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryStore.Application.Features.Products.Queries.GetAllProducts
{
    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, PaginatedResponse<GetAllProductsDto>>
    {
        private readonly ILibraryDbContext _context;
        public GetAllProductsQueryHandler(ILibraryDbContext context)
        {
            _context = context;
        }
        public async Task<PaginatedResponse<GetAllProductsDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {

            var query = _context.Products.AsNoTracking().Where(p => p.IsAvailable);

            if (request.CategoryId.HasValue)
            {
                query = query.Where(p =>
                    p.ProductCategories.Any(pc => pc.CategoryId == request.CategoryId)
                );
            }
            if (!string.IsNullOrEmpty(request.Search))
            {
                var search = request.Search.Trim();
                query = query.Where(p => EF.Functions.Like(p.Name, $"%{search}%"));
            }
            return await query
                .OrderBy(p => p.Id)
                .Select(p => new GetAllProductsDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    OldPrice = p.OldPrice,
                    PrimaryImage = p.Images
                    .Where(i => i.IsPrimary)
                    .Select(i => new GetProductImageDto
                    {
                        Id = i.Id,
                        ImageUrl = i.ImageUrl,
                        DisplayOrder = i.DisplayOrder,
                        IsPrimary = i.IsPrimary
                    }).FirstOrDefault(),
                    Categories = p.ProductCategories
                    .Select(pc => new GetCategoryDto
                    {
                        Id = pc.Category.Id,
                        Name = pc.Category.Name
                    }).ToList()
                }
                ).ToPaginatedResponse(request.PageNumber, request.PageSize, cancellationToken);

        }
    }
}
