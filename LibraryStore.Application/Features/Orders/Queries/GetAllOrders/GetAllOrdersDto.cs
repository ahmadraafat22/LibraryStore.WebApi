namespace LibraryStore.Application.Features.Orders.Queries.GetAllOrders
{
    public class GetAllOrdersDto
    {
        public Guid Id { get; set; }
        public decimal Total { get; set; }
        public string OrderNumber { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerAddress { get; set; }
        public string Status { get; set; }
        public string OrderItems { get; set; }
    }
}
