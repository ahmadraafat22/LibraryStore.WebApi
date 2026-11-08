namespace LibraryStore.Domain.Entities
{
    public class StoreSettings : BaseEntity
    {
        public string StoreName { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string? Facebook { get; set; }
        public string? Instagram { get; set; }
        public string? WhatsApp { get; set; }
        public string? TikTok { get; set; }
    }
}
