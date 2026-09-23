using LibraryStore.Application.Common.Interfaces;
using System.Text;

namespace LibraryStore.Infrastructure.Services
{
    public static class TelegramOrderMessageBuilder
    {
        public static string BuildOrderMessage(
            OrderTelegramNotification order,
            string? statusLabel = null)
        {
            var sb = new StringBuilder();

            var header = statusLabel is null
                ? $"📦 طلب جديد: {order.OrderNumber}"
                : $"{statusLabel}: {order.OrderNumber}";

            sb.AppendLine(header);
            sb.AppendLine();
            sb.AppendLine($"👤 العميل: {order.CustomerName}");
            sb.AppendLine($"📞 الهاتف: {order.CustomerPhone}");
            sb.AppendLine($"📍 العنوان: {order.CustomerAddress}");
            sb.AppendLine();
            sb.AppendLine("🛒 المنتجات:");

            foreach (var item in order.Items)
                sb.AppendLine($"• {item.ProductName} × {item.Quantity}");

            sb.AppendLine();
            sb.AppendLine($"💰 الإجمالي: {order.Total:N2} EGP");

            return sb.ToString();
        }
    }
}