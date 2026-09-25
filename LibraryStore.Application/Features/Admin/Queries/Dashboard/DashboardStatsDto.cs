namespace LibraryStore.Application.Features.Admin.Queries.Dashboard
{
    public class DashboardStatsDto
    {
        public decimal TotalSales { get; set; }
        public int TotalOrders { get; set; }
        public int ConfirmedOrders { get; set; }
        public int PendingOrders { get; set; }
        public int CancelledOrders { get; set; }
        public int AvailableProductsCount { get; set; }
        public List<BestSellingProductDto> BestSellingProducts { get; set; } = new();
    }
}
