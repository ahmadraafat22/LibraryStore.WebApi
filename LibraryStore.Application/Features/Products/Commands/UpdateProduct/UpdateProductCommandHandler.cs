using LibraryStore.Application.Common.Exceptions;
using LibraryStore.Domain.Entities;
using LibraryStore.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryStore.Application.Features.Products.Commands.UpdateProduct
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand>
    {
        private readonly ILibraryDbContext _context;
        public UpdateProductCommandHandler(ILibraryDbContext context)
        {
            _context = context;
        }
        public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _context.Products.Include(p => p.ProductCategories)
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (product == null)
            {
                throw new NotFoundException(nameof(Product), request.Id);
            }

            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;
            product.OldPrice = request.OldPrice;

            _context.ProductCategories.RemoveRange(product.ProductCategories);
            foreach (var id in request.CategoryIds)
            {
                product.ProductCategories.Add(new ProductCategory { CategoryId = id });
            }
            await _context.SaveChangesAsync(cancellationToken);

        }
    }
}
