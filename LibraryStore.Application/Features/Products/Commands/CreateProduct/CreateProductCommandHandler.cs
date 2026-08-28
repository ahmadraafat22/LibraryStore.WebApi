using LibraryStore.Domain.Entities;
using LibraryStore.Domain.Interfaces;
using MediatR;

namespace LibraryStore.Application.Features.Products.Commands.CreateProduct
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
    {
        private readonly ILibraryDbContext _context;
        public CreateProductCommandHandler(ILibraryDbContext context)
        {
            _context = context;
        }
        public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                OldPrice = request.OldPrice
            };

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync(cancellationToken);

            return product.Id;
        }
    }
}
