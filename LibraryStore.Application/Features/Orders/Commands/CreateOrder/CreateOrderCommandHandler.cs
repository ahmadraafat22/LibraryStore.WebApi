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
        public CreateOrderCommandHandler(ILibraryDbContext context, ICodeGeneratorService codeGeneratorService)
        {
            _context = context;
            _codeGeneratorService = codeGeneratorService;
        }

        public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            List<Guid> ProductIds = request.Items
                .Select(i => i.ProductId)
                .Distinct()
                .ToList();


            var products = await _context.Products
                .Where(p => ProductIds.Contains(p.Id))
                .ToListAsync(cancellationToken);
            if (products.Count != ProductIds.Count)
            {
                throw new Exception("Wrong Product Id");
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
            return order.Id;

        }
    }
}
