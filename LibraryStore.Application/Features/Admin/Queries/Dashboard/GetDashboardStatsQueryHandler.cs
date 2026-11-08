using LibraryStore.Domain.Common.Enums;
using LibraryStore.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryStore.Application.Features.Admin.Queries.Dashboard
{
    public class GetDashboardStatsQueryHandler : IRequestHandler<GetDashboardStatsQuery, DashboardStatsDto>
    {
        private readonly ILibraryDbContext _context;
        public GetDashboardStatsQueryHandler(ILibraryDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardStatsDto> Handle(
            GetDashboardStatsQuery request,
            CancellationToken cancellationToken)
        {
            DateTime? startDate = request.DashboardPeriod switch
            {
                DashboardPeriod.Day => DateTime.UtcNow.Date,
                DashboardPeriod.Week => DateTime.UtcNow.Date.AddDays(-7),
                DashboardPeriod.Month => DateTime.UtcNow.Date.AddDays(-30),
                null => null,   // مفيش فلتر
                _ => DateTime.UtcNow.Date.AddDays(-30)
            };

            var orderQuery = _context.Orders.AsNoTracking();
            if (startDate.HasValue)
                orderQuery = orderQuery.Where(o => o.CreatedAt >= startDate.Value);

            var orderStats = await orderQuery
                .GroupBy(o => 1)
                .Select(g => new
                {
                    TotalOrders = g.Count(),
                    ConfirmedOrders = g.Count(o => o.Status == OrderStatus.Confirmed),
                    PendingOrders = g.Count(o => o.Status == OrderStatus.Pending),
                    CancelledOrders = g.Count(o => o.Status == OrderStatus.Cancelled),
                    TotalSales = g.Where(o => o.Status == OrderStatus.Confirmed)
                                  .Sum(o => (decimal?)o.Total) ?? 0
                })
                .FirstOrDefaultAsync(cancellationToken);

            var availableProductsCount = await _context.Products
                .AsNoTracking()
                .CountAsync(p => p.IsAvailable, cancellationToken);

            var bestSellersQuery = _context.OrderItems
                .AsNoTracking()
                .Where(oi => oi.Order.Status == OrderStatus.Confirmed);

            if (startDate.HasValue)
                bestSellersQuery = bestSellersQuery
                    .Where(oi => oi.Order.CreatedAt >= startDate.Value);

            var bestSellers = await bestSellersQuery
                .GroupBy(oi => new { oi.ProductId, oi.ProductName })
                .Select(g => new BestSellingProductDto
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.ProductName,
                    TotalQuantitySold = g.Sum(oi => oi.Quantity)
                })
                .OrderByDescending(x => x.TotalQuantitySold)
                .Take(10)
                .ToListAsync(cancellationToken);

            return new DashboardStatsDto
            {
                TotalOrders = orderStats?.TotalOrders ?? 0,
                ConfirmedOrders = orderStats?.ConfirmedOrders ?? 0,
                PendingOrders = orderStats?.PendingOrders ?? 0,
                CancelledOrders = orderStats?.CancelledOrders ?? 0,
                TotalSales = orderStats?.TotalSales ?? 0,
                AvailableProductsCount = availableProductsCount,
                BestSellingProducts = bestSellers
            };
        }
    }
}
