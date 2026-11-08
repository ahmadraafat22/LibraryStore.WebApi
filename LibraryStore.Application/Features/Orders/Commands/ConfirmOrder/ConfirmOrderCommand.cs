using LibraryStore.Application.Common.Interfaces;
using MediatR;

namespace LibraryStore.Application.Features.Orders.Commands.ConfirmOrder
{
    public class ConfirmOrderCommand : IRequest<OrderTelegramNotification>
    {
        public Guid OrderId { get; set; }
        public ConfirmOrderCommand(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}
