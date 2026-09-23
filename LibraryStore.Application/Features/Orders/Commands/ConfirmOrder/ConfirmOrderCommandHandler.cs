using LibraryStore.Application.Common.Exceptions;
using LibraryStore.Application.Common.Interfaces;
using LibraryStore.Domain.Common.Enums;
using LibraryStore.Domain.Entities;
using LibraryStore.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryStore.Application.Features.Orders.Commands.ConfirmOrder
{
    public class ConfirmOrderCommandHandler : IRequestHandler<ConfirmOrderCommand, OrderTelegramNotification>
    {
        private readonly ILibraryDbContext _context;
        public ConfirmOrderCommandHandler(ILibraryDbContext context)
        {
            _context = context;
        }
        public async Task<OrderTelegramNotification> Handle(ConfirmOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(
                    order => order.Id == request.OrderId,
                    cancellationToken);

            if (order is null)
            {
                throw new NotFoundException(nameof(Order), request.OrderId);
            }

            if (order.Status != OrderStatus.Pending)
            {
                throw new BusinessRuleException(
                    "لا يمكن تأكيد الطلب لأن حالته ليست قيد الانتظار.");
            }
            order.Status = OrderStatus.Confirmed;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
            var orderItems = order.OrderItems.Select(i => new OrderTelegramItem(i.ProductName, i.Quantity))
                .ToList();
            return new OrderTelegramNotification(order.Id, order.OrderNumber, order.CustomerName, order.CustomerPhone, order.CustomerAddress, order.Total, orderItems);
        }
    }
}
