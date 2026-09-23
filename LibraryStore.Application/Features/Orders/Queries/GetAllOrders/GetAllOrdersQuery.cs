using MediatR;

namespace LibraryStore.Application.Features.Orders.Queries.GetAllOrders
{
    public class GetAllOrdersQuery : IRequest<List<GetAllOrdersDto>>
    {

    }
}
