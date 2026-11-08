using LibraryStore.Application.Common.Exceptions;
using LibraryStore.Domain.Entities;
using LibraryStore.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryStore.Application.Features.Orders.Queries.GetOrderById
{
    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, GetOrderByIdDto>
    {
        private readonly ILibraryDbContext _context;
        public GetOrderByIdQueryHandler(ILibraryDbContext context)
        {
            _context = context;
        }

        public async Task<GetOrderByIdDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var order = await _context.Orders
                .Where(o => o.Id == request.Id)
                .Select(o => new GetOrderByIdDto
                {
                    Id = o.Id,
                    OrderNumber = o.OrderNumber,
                    CustomerAddress = o.CustomerAddress,
                    CustomerName = o.CustomerName,
                    CustomerPhone = o.CustomerPhone,
                    Status = o.Status,
                    OrderItems = o.OrderItems
                    .Select(oi => new OrderItemDetailDto
                    {
                        ProductName = oi.ProductName,
                        UnitPrice = oi.UnitPrice,
                        Quantity = oi.Quantity,
                        LineTotal = oi.LineTotal
                    }).ToList(),
                    Total = o.Total,
                    UpdatedAt = o.UpdatedAt,
                    CreatedAt = o.CreatedAt
                }).FirstOrDefaultAsync(cancellationToken);

            if (order == null)
            {
                throw new NotFoundException(nameof(Order), request.Id);
            }
            return order;
        }
    }
}
