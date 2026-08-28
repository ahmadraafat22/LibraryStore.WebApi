using LibraryStore.Application.Common.Exceptions;
using LibraryStore.Application.Features.Products.Queries.GetAllProducts;
using LibraryStore.Domain.Entities;
using LibraryStore.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryStore.Application.Features.Products.Queries.GetProductById
{
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, GetProductDto>
    {
        private readonly ILibraryDbContext _context;
        public GetProductByIdQueryHandler(ILibraryDbContext context)
        {
            _context = context;
        }

        public async Task<GetProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
                .Select(p => new GetProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    OldPrice = p.OldPrice
                })
                .FirstOrDefaultAsync(p => p.Id == request.Id);

            if (product == null)
            {
                throw new NotFoundException(nameof(Product), request.Id);
            }

            return product;
        }
    }
}
