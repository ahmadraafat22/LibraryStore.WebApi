using LibraryStore.Application.Common.Exceptions;
using LibraryStore.Application.Common.Interfaces;
using LibraryStore.Domain.Common.Enums;
using LibraryStore.Domain.Entities;
using LibraryStore.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace LibraryStore.Application.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
    {
        private readonly ILibraryDbContext _context;
        private ICodeGeneratorService _codeGeneratorService;
        private readonly ITelegramOrderNotifier _telegramOrderNotifier;
        public CreateOrderCommandHandler(ILibraryDbContext context, ICodeGeneratorService codeGeneratorService, ITelegramOrderNotifier telegramOrderNotifier)
        {
            _context = context;
            _codeGeneratorService = codeGeneratorService;
            _telegramOrderNotifier = telegramOrderNotifier;
        }

        public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            List<Guid> ProductIds = request.Items
                .Select(i => i.ProductId)
                .Distinct()
                .ToList();


            var products = await _context.Products
                .Where(p => ProductIds.Contains(p.Id) && p.IsAvailable)
                .ToListAsync(cancellationToken);
            if (products.Count != ProductIds.Count)
            {
                throw new BusinessRuleException("One or more products are unAvailble or do not exists !");
            }
            var orderItems = new List<OrderItem>();

            foreach (var product in products)
            {
                var matchedItem = request.Items
                    .FirstOrDefault(i => i.ProductId == product.Id);
                var orderItem = new OrderItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    UnitPrice = product.Price,
                    Quantity = matchedItem.Quantity,
                    LineTotal = product.Price * matchedItem.Quantity
                };
                orderItems.Add(orderItem);
            }
            var order = new Order
            {
                CustomerName = request.CustomerName,
                CustomerAddress = request.CustomerAddress,
                CustomerPhone = request.CustomerPhone,
                OrderItems = orderItems,
                Status = OrderStatus.Pending,
                Total = orderItems.Sum(oi => oi.LineTotal),
                OrderNumber = await _codeGeneratorService.GenerateCodeAsync(CodeType.Order)
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync(cancellationToken);

            await SendTelegramWithRetryAsync(order, cancellationToken);

            return order.Id;

        }
        private async Task SendTelegramWithRetryAsync(
            Order order,
            CancellationToken cancellationToken)
        {
            const int maxAttempts = 3;
            var delays = new[] { 2, 5 };
            var orderItems = order.OrderItems
                .Select(i => new OrderTelegramItem(i.ProductName, i.Quantity)).ToList();
            var notification = new OrderTelegramNotification(order.Id, order.OrderNumber, order.CustomerName, order.CustomerPhone, order.CustomerAddress, order.Total, orderItems);

            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    await _telegramOrderNotifier.NotifyNewOrderAsync(notification, cancellationToken);
                    return;
                }
                catch
                {
                    if (attempt == maxAttempts) return;

                    await Task.Delay(TimeSpan.FromSeconds(delays[attempt - 1]), cancellationToken);
                }
            }
        }
    }
}
