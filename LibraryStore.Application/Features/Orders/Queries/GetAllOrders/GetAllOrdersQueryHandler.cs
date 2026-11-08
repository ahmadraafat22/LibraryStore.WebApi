using LibraryStore.Application.Common.Extentions;
using LibraryStore.Application.Common.Responses;
using LibraryStore.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryStore.Application.Features.Orders.Queries.GetAllOrders
{
    public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, PaginatedResponse<GetAllOrdersDto>>
    {
        private readonly ILibraryDbContext _context;
        public GetAllOrdersQueryHandler(ILibraryDbContext context)
        {
            _context = context;
        }
        public async Task<PaginatedResponse<GetAllOrdersDto>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Orders.AsNoTracking();

            if (request.Status.HasValue)
            {
                query = query.Where(o => o.Status == request.Status.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim();
                var pattern = $"%{search}%";

                query = query.Where(o =>
                    EF.Functions.Like(o.OrderNumber, pattern) ||
                    EF.Functions.Like(o.CustomerPhone, pattern) ||
                    EF.Functions.Like(o.CustomerName, pattern));
            }
            return await query
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new GetAllOrdersDto
            {
                Id = o.Id,
                CustomerName = o.CustomerName,
                CustomerAddress = o.CustomerAddress,
                CustomerPhone = o.CustomerPhone,
                OrderNumber = o.OrderNumber,
                Status = o.Status.ToString(),
                ItemsCount = o.OrderItems.Count(),
                Total = o.Total,
                CreatedAt = o.CreatedAt.ToLocalTime()
            }).ToPaginatedResponse(request.PageNumber, request.PageSize, cancellationToken);
        }
    }
}
