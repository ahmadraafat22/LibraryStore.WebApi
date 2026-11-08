using LibraryStore.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryStore.Application.Features.StoreSettings.Queries.GetStoreSettings
{
    public class GetStoreSettingsQueryHandler : IRequestHandler<GetStoreSettingsQuery, GetStoreSettingsDto>
    {
        private readonly ILibraryDbContext _context;
        public GetStoreSettingsQueryHandler(ILibraryDbContext context)
        {
            _context = context;
        }

        public async Task<GetStoreSettingsDto> Handle(
            GetStoreSettingsQuery request,
            CancellationToken cancellationToken)
        {
            var settings = await _context.StoreSettings
                .AsNoTracking()
                .Select(s => new GetStoreSettingsDto
                {
                    StoreName = s.StoreName,
                    Address = s.Address,
                    Phone = s.Phone,
                    Facebook = s.Facebook,
                    WhatsApp = s.WhatsApp,
                    Instagram = s.Instagram,
                    TikTok = s.TikTok
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (settings is null)
                throw new Exception("Store settings not Found");

            return settings;
        }
    }
}
