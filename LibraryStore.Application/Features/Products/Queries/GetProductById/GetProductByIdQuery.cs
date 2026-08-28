using LibraryStore.Application.Features.Products.Queries.GetAllProducts;
using MediatR;

namespace LibraryStore.Application.Features.Products.Queries.GetProductById
{
    public class GetProductByIdQuery : IRequest<GetProductDto>
    {
        public Guid Id { get; set; }
        public GetProductByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
