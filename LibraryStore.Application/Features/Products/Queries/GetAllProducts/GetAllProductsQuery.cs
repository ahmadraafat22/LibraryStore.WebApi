using MediatR;

namespace LibraryStore.Application.Features.Products.Queries.GetAllProducts
{
    public class GetAllProductsQuery : IRequest<ICollection<GetAllProductsDto>>
    {

    }
}
