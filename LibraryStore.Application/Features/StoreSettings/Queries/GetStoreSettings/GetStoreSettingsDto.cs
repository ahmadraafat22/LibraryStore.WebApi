namespace LibraryStore.Application.Features.StoreSettings.Queries.GetStoreSettings
{
    public class GetStoreSettingsDto
    {
        public string StoreName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string? Facebook { get; set; }
        public string? Instagram { get; set; }
        public string? WhatsApp { get; set; }
        public string? TikTok { get; set; }
    }
}
