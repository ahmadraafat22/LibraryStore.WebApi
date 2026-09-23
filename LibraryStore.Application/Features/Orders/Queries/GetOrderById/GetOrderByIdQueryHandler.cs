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
                    OrderNumber = o.OrderNumber,
                    CustomerAddress = o.CustomerAddress,
                    CustomerName = o.CustomerName,
                    CustomerPhone = o.CustomerPhone,
                    Status = o.Status.ToString(),
                    OrderItems = string.Join("\n",
                        o.OrderItems.Select(oi => $"{oi.ProductName} x {oi.Quantity}")
                    ),
                    Total = o.Total
                }).FirstOrDefaultAsync(cancellationToken);

            if (order == null)
            {
                throw new NotFoundException(nameof(Order), request.Id);
            }
            return order;
        }
    }
}
