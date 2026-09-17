using LibraryStore.Application.Features.ProductImages.Queries;
using LibraryStore.Application.Features.Products.Queries.GetAllProducts;
using LibraryStore.Application.Features.Products.Queries.GetProductById;
using LibraryStore.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryStore.Application.Features.Products.Queries.GetNew_Arrivals
{
    public class GetNewArrivalsQueryHandler : IRequestHandler<GetNewArrivalsQuery, List<GetAllProductsDto>>
    {
        private readonly ILibraryDbContext _context;
        public GetNewArrivalsQueryHandler(ILibraryDbContext context)
        {
            _context = context;
        }

        public async Task<List<GetAllProductsDto>> Handle(GetNewArrivalsQuery request, CancellationToken cancellationToken)
        {
            List<GetAllProductsDto> newArrivals = await _context.Products
                .AsNoTracking()
                .Where(p => p.IsAvailable)
                .OrderByDescending(p => p.CreatedAt)
                .Take(6)
                .Select(p => new GetAllProductsDto
                {
                    Id = p.Id,
                    Price = p.Price,
                    OldPrice = p.OldPrice,
                    Name = p.Name,
                    PrimaryImage = p.Images
                    .Where(i => i.IsPrimary)
                    .Select(pi => new GetProductImageDto
                    {
                        Id = pi.Id,
                        ImageUrl = pi.ImageUrl,
                        DisplayOrder = pi.DisplayOrder,
                        IsPrimary = pi.IsPrimary
                    }).FirstOrDefault(),
                    Categories = p.ProductCategories
                    .Select(pc => new GetCategoryDto
                    {
                        Id = pc.CategoryId,
                        Name = pc.Category.Name
                    })
                    .ToList()
                }).ToListAsync(cancellationToken);

            return newArrivals;
        }
    }
}
