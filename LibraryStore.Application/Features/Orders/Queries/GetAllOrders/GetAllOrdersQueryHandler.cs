using LibraryStore.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryStore.Application.Features.Orders.Queries.GetAllOrders
{
    public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, List<GetAllOrdersDto>>
    {
        private readonly ILibraryDbContext _context;
        public GetAllOrdersQueryHandler(ILibraryDbContext context)
        {
            _context = context;
        }
        public async Task<List<GetAllOrdersDto>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
        {
            var orders = await _context.Orders
                .AsNoTracking()
                .Select(o => new GetAllOrdersDto
                {
                    Id = o.Id,
                    CustomerName = o.CustomerName,
                    CustomerAddress = o.CustomerAddress,
                    CustomerPhone = o.CustomerPhone,
                    OrderNumber = o.OrderNumber,
                    Status = o.Status.ToString(),
                    OrderItems = string.Join("\n",
                        o.OrderItems.Select(oi => $"{oi.ProductName} x {oi.Quantity}")
                    ),
                    Total = o.Total
                }).ToListAsync(cancellationToken);
            return orders;
        }
    }
}
