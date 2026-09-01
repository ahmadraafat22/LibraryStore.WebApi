using MediatR;

namespace LibraryStore.Application.Features.Products.Commands.SoftDeleteProduct
{
    public class SoftDeleteProductCommand : IRequest
    {
        public Guid Id { get; set; }
        public SoftDeleteProductCommand(Guid id)
        {
            Id = id;
        }
    }
}
