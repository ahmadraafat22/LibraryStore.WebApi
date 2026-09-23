using System.ComponentModel.DataAnnotations;

namespace LibraryStore.Infrastructure.Settings
{
    public sealed class TelegramOptions
    {
        public const string SectionName = "Telegram";
        [Required]
        public string BotToken { get; init; } = string.Empty;
        public long AdminChatId { get; init; }
        public long AdminUserId { get; init; }
        [Required]
        public string WebhookSecret { get; init; } = string.Empty;
        [Required]
        public string WebhookUrl { get; init; } = string.Empty;
    }
}
