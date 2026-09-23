using LibraryStore.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace LibraryStore.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TelegramController : ControllerBase
    {
        private readonly TelegramWebhookHandler _telegramWebhookHandler;
        public TelegramController(TelegramWebhookHandler telegramWebhookHandler)
        {
            _telegramWebhookHandler = telegramWebhookHandler;
        }

        [AllowAnonymous]
        [HttpPost("Webhook")]
        public async Task<IActionResult> HandleWebhook([FromBody] Update update,
           [FromHeader(Name = "X-Telegram-Bot-Api-Secret-Token")] string? webhookSecret,
            CancellationToken cancellation)
        {
            await _telegramWebhookHandler.HandleAsync(update, webhookSecret, cancellation);
            return Ok();
        }

    }
}
