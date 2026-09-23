namespace LibraryStore.Application.Common.Interfaces
{
    public interface ITelegramOrderNotifier
    {
        Task NotifyNewOrderAsync(
            OrderTelegramNotification order,
            CancellationToken cancellationToken = default);
    }
    public sealed record OrderTelegramNotification(
            Guid OrderId,
            string OrderNumber,
            string CustomerName,
            string CustomerPhone,
            string CustomerAddress,
            decimal Total,
            ICollection<OrderTelegramItem> Items
        );
    public sealed record OrderTelegramItem(
            string ProductName,
            int Quantity
        );
}
