using LibraryStore.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryStore.Application.Features.Products.Queries.GetAllProducts
{
    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, ICollection<GetProductDto>>
    {
        private readonly ILibraryDbContext _context;
        public GetAllProductsQueryHandler(ILibraryDbContext context)
        {
            _context = context;
        }
        public async Task<ICollection<GetProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {

            return await _context.Products.AsNoTracking()
                .Where(p => p.IsAvailable)
                .Select(p => new GetProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    OldPrice = p.OldPrice
                }
                ).ToListAsync(cancellationToken);

        }
    }
}
