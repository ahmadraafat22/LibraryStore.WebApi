using MediatR;

namespace LibraryStore.Application.Features.Orders.Queries.GetOrderById
{
    public class GetOrderByIdQuery : IRequest<GetOrderByIdDto>
    {
        public Guid Id { get; set; }
        public GetOrderByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
