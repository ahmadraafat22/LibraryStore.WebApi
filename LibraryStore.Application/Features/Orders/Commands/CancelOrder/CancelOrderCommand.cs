using LibraryStore.Application.Common.Interfaces;
using MediatR;

namespace LibraryStore.Application.Features.Orders.Commands.CancelOrder
{
    public class CancelOrderCommand : IRequest<OrderTelegramNotification>
    {
        public Guid OrderId { get; set; }
        public CancelOrderCommand(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}
