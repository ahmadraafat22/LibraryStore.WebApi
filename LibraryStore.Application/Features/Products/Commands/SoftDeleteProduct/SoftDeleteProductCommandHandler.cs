using LibraryStore.Application.Common.Exceptions;
using LibraryStore.Domain.Entities;
using LibraryStore.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryStore.Application.Features.Products.Commands.SoftDeleteProduct
{
    public class SoftDeleteProductCommandHandler : IRequestHandler<SoftDeleteProductCommand>
    {
        private readonly ILibraryDbContext _context;
        public SoftDeleteProductCommandHandler(ILibraryDbContext context)
        {
            _context = context;
        }
        public async Task Handle(SoftDeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
            if (product == null)
            {
                throw new NotFoundException(nameof(Product), request.Id);
            }
            product.IsDeleted = true;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
