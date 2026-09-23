using LibraryStore.Application.Common.Exceptions;
using LibraryStore.Application.Common.Interfaces;
using LibraryStore.Domain.Common.Enums;
using LibraryStore.Domain.Entities;
using LibraryStore.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryStore.Application.Features.Orders.Commands.CancelOrder
{
    public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, OrderTelegramNotification>
    {
        private readonly ILibraryDbContext _context;
        public CancelOrderCommandHandler(ILibraryDbContext context)
        {
            _context = context;
        }
        public async Task<OrderTelegramNotification> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

            if (order is null)
            {
                throw new NotFoundException(nameof(Order), request.OrderId);
            }

            if (order.Status != OrderStatus.Pending)
            {
                throw new BusinessRuleException(
                    "لا يمكن الغاء الطلب لأن حالته ليست قيد الانتظار.");
            }
            order.Status = OrderStatus.Cancelled;
            await _context.SaveChangesAsync(cancellationToken);
            var orderItems = order.OrderItems.Select(i => new OrderTelegramItem(i.ProductName, i.Quantity))
                .ToList();

            return new OrderTelegramNotification(order.Id, order.OrderNumber, order.CustomerName, order.CustomerPhone, order.CustomerAddress, order.Total, orderItems);
        }
    }
}
