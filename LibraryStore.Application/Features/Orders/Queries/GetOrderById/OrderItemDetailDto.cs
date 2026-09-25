namespace LibraryStore.Application.Features.Orders.Queries.GetOrderById
{
    public class OrderItemDetailDto
    {
        public string ProductName { get; set; } = null!;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal { get; set; }
    }
}
