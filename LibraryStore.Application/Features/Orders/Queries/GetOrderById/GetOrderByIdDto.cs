using LibraryStore.Domain.Common.Enums;

namespace LibraryStore.Application.Features.Orders.Queries.GetOrderById
{
    public class GetOrderByIdDto
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerPhone { get; set; }
        public OrderStatus Status { get; set; }
        public List<OrderItemDetailDto> OrderItems { get; set; }
        public decimal Total { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
