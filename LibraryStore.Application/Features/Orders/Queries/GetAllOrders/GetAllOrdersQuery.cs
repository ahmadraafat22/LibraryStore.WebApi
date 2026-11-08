using LibraryStore.Application.Common.Responses;
using LibraryStore.Domain.Common.Enums;
using MediatR;

namespace LibraryStore.Application.Features.Orders.Queries.GetAllOrders
{
    public class GetAllOrdersQuery : IRequest<PaginatedResponse<GetAllOrdersDto>>
    {
        public string? Search { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 3;
        public OrderStatus? Status { get; set; }
    }
}
