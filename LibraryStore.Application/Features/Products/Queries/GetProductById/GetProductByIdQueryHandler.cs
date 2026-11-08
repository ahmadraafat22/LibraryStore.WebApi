using LibraryStore.Application.Common.Exceptions;
using LibraryStore.Application.Features.ProductImages.Queries;
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
                .Include(p => p.Images)
                .Include(p => p.ProductCategories).ThenInclude(pc => pc.Category)
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (product == null)
            {
                throw new NotFoundException(nameof(Product), request.Id);
            }

            return new GetProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                OldPrice = product.OldPrice,
                Categories = product.ProductCategories.Select(pc => new GetCategoryDto
                {
                    Id = pc.Category.Id,
                    Name = pc.Category.Name
                }).ToList(),
                Images = product.Images
                .OrderBy(i => i.DisplayOrder)
                .Select(i => new GetProductImageDto
                {
                    Id = i.Id,
                    ImageUrl = i.ImageUrl,
                    IsPrimary = i.IsPrimary,
                    DisplayOrder = i.DisplayOrder
                }).ToList()
            };
        }
    }
}
