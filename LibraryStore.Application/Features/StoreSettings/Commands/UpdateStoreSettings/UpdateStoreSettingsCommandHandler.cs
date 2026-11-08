using LibraryStore.Application.Common.Exceptions;
using LibraryStore.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryStore.Application.Features.StoreSettings.Commands.UpdateStoreSettings
{
    public class UpdateStoreSettingsCommandHandler : IRequestHandler<UpdateStoreSettingsCommand>
    {
        private readonly ILibraryDbContext _context;
        public UpdateStoreSettingsCommandHandler(ILibraryDbContext context)
        {
            _context = context;
        }

        public async Task Handle(UpdateStoreSettingsCommand request, CancellationToken cancellationToken)
        {
            var settings = await _context.StoreSettings.FirstOrDefaultAsync(cancellationToken);
            if (settings is null)
                throw new NotFoundException(nameof(StoreSettings), request.StoreName);

            settings.StoreName = request.StoreName;
            settings.Phone = request.Phone;
            settings.Address = request.Address;
            settings.Instagram = request.Instagram;
            settings.Facebook = request.Facebook;
            settings.WhatsApp = request.WhatsApp;
            settings.TikTok = request.TikTok;
            settings.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
