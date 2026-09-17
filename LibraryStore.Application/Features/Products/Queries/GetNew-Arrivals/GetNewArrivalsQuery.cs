using LibraryStore.Application.Features.Products.Queries.GetAllProducts;
using MediatR;

namespace LibraryStore.Application.Features.Products.Queries.GetNew_Arrivals
{
    public class GetNewArrivalsQuery : IRequest<List<GetAllProductsDto>>
    {
    }
}
