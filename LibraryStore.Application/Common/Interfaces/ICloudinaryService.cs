using Microsoft.AspNetCore.Http;

namespace LibraryStore.Application.Common.Interfaces
{
    public interface ICloudinaryService
    {
        Task<ImageUploadResult> UploadImageAsync(
            IFormFile image,
            CancellationToken cancellationToken = default);

        Task DeleteImageAsync(
            string publicId,
            CancellationToken cancellationToken = default);
    }
}
