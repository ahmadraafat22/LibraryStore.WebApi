using LibraryStore.Application.Features.ProductImages.Queries;
using LibraryStore.Application.Features.Products.Queries.GetProductById;
using LibraryStore.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryStore.Application.Features.Products.Queries.GetAllProducts
{
    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, ICollection<GetAllProductsDto>>
    {
        private readonly ILibraryDbContext _context;
        public GetAllProductsQueryHandler(ILibraryDbContext context)
        {
            _context = context;
        }
        public async Task<ICollection<GetAllProductsDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {

            return await _context.Products.AsNoTracking()
                .Where(p => p.IsAvailable)
                .Select(p => new GetAllProductsDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    OldPrice = p.OldPrice,
                    PrimaryImage = p.Images
                    .Select(i => new GetProductImageDto
                    {
                        Id = i.Id,
                        ImageUrl = i.ImageUrl,
                        DisplayOrder = i.DisplayOrder,
                        IsPrimary = i.IsPrimary
                    }).FirstOrDefault(i => i.IsPrimary),
                    Categories = p.ProductCategories
                    .Select(pc => new GetCategoryDto
                    {
                        Id = pc.Category.Id,
                        Name = pc.Category.Name
                    }).ToList()
                }
                ).ToListAsync(cancellationToken);

        }
    }
}
