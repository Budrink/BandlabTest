using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Png;
using Microsoft.Extensions.Options;
using System.Linq;
using ImageCommentApp.Configuration;

namespace ImageCommentApp.Services
{

    public class ImageService : IImageService
    {
        private readonly string _imageDirectory;

        public ImageService(IOptions<ImageSettings> imageSettings)
        {
            _imageDirectory = imageSettings.Value.ImageDirectory;

            // Создаем директорию, если она не существует
            if (!Directory.Exists(_imageDirectory))
            {
                Directory.CreateDirectory(_imageDirectory);
            }
        }

        public async Task<(string originalFilePath, string jpgFilePath)> SaveAndProcessImageAsync(Stream imageStream, string originalFileName)
        {
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalFileName);
            var fileExtension = Path.GetExtension(originalFileName).ToLower();

            // Проверяем, поддерживается ли формат
            var allowedExtensions = new[] { ".png", ".jpg", ".bmp" };
            if (!allowedExtensions.Contains(fileExtension))
            {
                throw new InvalidOperationException("Unsupported image format.");
            }

            // Сохраняем в оригинальном формате
            var originalSavedFileName = $"{fileNameWithoutExtension}_{Guid.NewGuid()}{fileExtension}";
            var originalFilePath = Path.Combine(_imageDirectory, originalSavedFileName);

            using (var originalFileStream = new FileStream(originalFilePath, FileMode.Create))
            {
                await imageStream.CopyToAsync(originalFileStream);
            }

            // Конвертируем в .jpg
            var jpgFileName = $"{fileNameWithoutExtension}_{Guid.NewGuid()}.jpg";
            var jpgFilePath = Path.Combine(_imageDirectory, jpgFileName);

            imageStream.Position = 0; // Сбрасываем позицию потока
            using (var image = await Image.LoadAsync(imageStream))
            {
                image.Mutate(x => x.Resize(600, 600));
                await image.SaveAsync(jpgFilePath, new JpegEncoder());
            }

            return (originalFilePath, jpgFilePath);
        }
    }
}