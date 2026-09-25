namespace LibraryStore.Application.Features.Admin.Queries.Dashboard
{
    public class BestSellingProductDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public int TotalQuantitySold { get; set; }
    }
}
