using MediatR;

namespace LibraryStore.Application.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderCommand : IRequest<Guid>
    {
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerAddress { get; set; }
        public IList<CreateOrderItemDto> Items { get; set; }

    }
}
