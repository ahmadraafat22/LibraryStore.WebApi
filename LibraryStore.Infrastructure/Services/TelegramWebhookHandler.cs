using LibraryStore.Application.Common.Exceptions;
using LibraryStore.Application.Common.Interfaces;
using LibraryStore.Application.Features.Orders.Commands.CancelOrder;
using LibraryStore.Application.Features.Orders.Commands.ConfirmOrder;
using LibraryStore.Infrastructure.Settings;
using MediatR;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace LibraryStore.Infrastructure.Services
{
    public class TelegramWebhookHandler
    {
        private readonly IMediator _mediator;
        private readonly ITelegramBotClient _telegramBotClient;
        private readonly TelegramOptions _telegramOptions;
        public TelegramWebhookHandler(IMediator mediator,
            ITelegramBotClient telegramBotClient,
            IOptions<TelegramOptions> telegramOptions)
        {
            _mediator = mediator;
            _telegramBotClient = telegramBotClient;
            _telegramOptions = telegramOptions.Value;
        }
        public async Task HandleAsync(Update update,
            string? webhookSecret,
            CancellationToken cancellationToken = default)
        {
            if (!string.Equals(webhookSecret, _telegramOptions.WebhookSecret, StringComparison.Ordinal))
            {
                return;
            }

            if (update.Type != UpdateType.CallbackQuery || update.CallbackQuery?.Data is null)
            {
                return;
            }
            var callbackQuery = update.CallbackQuery;
            if (callbackQuery.From.Id != _telegramOptions.AdminUserId)
            {
                return;
            }
            var callbackDataParts = callbackQuery.Data.Split(':');
            if (callbackDataParts.Length != 2 || !Guid.TryParse(callbackDataParts[1], out var orderId))
            {
                return;
            }

            try
            {
                switch (callbackDataParts[0])
                {
                    case "confirm":
                        {
                            var order = await _mediator.Send(
                                new ConfirmOrderCommand(orderId), cancellationToken);

                            await EditOrderMessageAsync(
                                callbackQuery, order, "✅ طلب مؤكد", cancellationToken);

                            await _telegramBotClient.AnswerCallbackQuery(
                                callbackQuery.Id, cancellationToken: cancellationToken);
                            break;
                        }
                    case "cancel":
                        {
                            var order = await _mediator.Send(
                                new CancelOrderCommand(orderId), cancellationToken);

                            await EditOrderMessageAsync(
                                callbackQuery, order, "❌ طلب ملغي", cancellationToken);

                            await _telegramBotClient.AnswerCallbackQuery(
                                callbackQuery.Id, cancellationToken: cancellationToken);
                            break;
                        }
                }
            }
            catch (BusinessRuleException ex)
            {
                await _telegramBotClient.AnswerCallbackQuery(
                    callbackQuery.Id, ex.Message,
                    showAlert: true, cancellationToken: cancellationToken);
            }
            catch (NotFoundException ex)
            {
                await _telegramBotClient.AnswerCallbackQuery(
                    callbackQuery.Id, ex.Message,
                    showAlert: true, cancellationToken: cancellationToken);
            }
        }
        public async Task RegisterWebhookAsync(
            CancellationToken cancellationToken = default)
        {
            await _telegramBotClient.SetWebhook(
                url: _telegramOptions.WebhookUrl,
                secretToken: _telegramOptions.WebhookSecret,
                allowedUpdates: new[] { UpdateType.CallbackQuery },
                cancellationToken: cancellationToken);
        }
        private async Task EditOrderMessageAsync(
            CallbackQuery callbackQuery,
            OrderTelegramNotification order,
            string statusLabel,
            CancellationToken cancellationToken)
        {
            if (callbackQuery.Message is null) return;

            var newText = TelegramOrderMessageBuilder.BuildOrderMessage(order, statusLabel);

            try
            {
                await _telegramBotClient.EditMessageText(
                    chatId: callbackQuery.Message.Chat.Id,
                    messageId: callbackQuery.Message.MessageId,
                    text: newText,
                    replyMarkup: null,
                    cancellationToken: cancellationToken);
            }
            catch (Telegram.Bot.Exceptions.ApiRequestException)
            {
                // fallback: الرسالة قديمة أو مش قابلة للتعديل
                await _telegramBotClient.SendMessage(
                    chatId: callbackQuery.Message.Chat.Id,
                    text: newText,
                    cancellationToken: cancellationToken);
            }
        }
    }
}
