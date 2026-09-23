using LibraryStore.Application.Common.Interfaces;
using LibraryStore.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
namespace LibraryStore.Infrastructure.Services
{
    public class TelegramOrderNotifier : ITelegramOrderNotifier
    {
        private readonly ITelegramBotClient _telegramBotClient;
        private readonly IOptions<TelegramOptions> _options;
        public TelegramOrderNotifier(ITelegramBotClient telegramBotClient, IOptions<TelegramOptions> options)
        {
            _telegramBotClient = telegramBotClient;
            _options = options;
        }
        public async Task NotifyNewOrderAsync(OrderTelegramNotification order, CancellationToken cancellationToken = default)
        {
            var message = TelegramOrderMessageBuilder.BuildOrderMessage(order);
            var keyboard = BuildOrderKeyboard(order.OrderId);
            await _telegramBotClient.SendMessage(
                    chatId: _options.Value.AdminChatId,
                    text: message,
                    replyMarkup: keyboard,
                    cancellationToken: cancellationToken
                );
        }
        private static InlineKeyboardMarkup BuildOrderKeyboard(Guid orderId)
        {
            return new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("✅ Confirm Order",$"confirm:{orderId}"),
                    InlineKeyboardButton.WithCallbackData("❌ Cancel Order",$"cancel:{orderId}"),
                }
            });
        }
    }
}
