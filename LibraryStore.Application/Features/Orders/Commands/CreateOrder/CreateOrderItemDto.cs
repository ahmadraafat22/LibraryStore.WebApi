namespace LibraryStore.Application.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderItemDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
