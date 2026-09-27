using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using LibraryStore.Application.Common.Interfaces;
using LibraryStore.Infrastructure.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using ImageUploadResult = LibraryStore.Application.Common.Interfaces.ImageUploadResult;

namespace LibraryStore.Infrastructure.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IOptions<CloudinaryOptions> options)
        {
            var settings = options.Value;

            var account = new Account(
                settings.CloudName,
                settings.ApiKey,
                settings.ApiSecret);

            _cloudinary = new Cloudinary(account);
        }

        public async Task<ImageUploadResult> UploadImageAsync(
            IFormFile image,
            CancellationToken cancellationToken = default)
        {
            if (image == null || image.Length == 0)
                throw new ArgumentException("Image is required.");

            await using var stream = image.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(image.FileName, stream),
                Folder = "librarystore/products"
            };

            var result = await _cloudinary.UploadAsync(
                uploadParams,
                cancellationToken);

            if (result.Error != null)
                throw new Exception(result.Error.Message);

            return new ImageUploadResult
            {
                Url = result.SecureUrl.ToString(),
                PublicId = result.PublicId
            };
        }

        public async Task DeleteImageAsync(
            string publicId,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(publicId))
                return;

            var deleteParams = new DeletionParams(publicId);

            var result = await _cloudinary.DestroyAsync(deleteParams);

            if (result.Error != null)
                throw new Exception(result.Error.Message);
        }
    }
}
