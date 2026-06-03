using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using RepositoryLibrary.Features.Images.Interfaces;


namespace RepositoryLibrary.Features.Images.Services
{
    public class ImageService : IImageService
    {
        private readonly ILogger<ImageService> _logger;

        public ImageService(ILogger<ImageService> logger)
        {
            _logger = logger;
        }

        public async Task<string> SaveImageAsync(
            IBrowserFile file,
            string folder,
            string fileName)
        {
            const long maxFileSize = 5 * 1024 * 1024;

            string uploadsFolder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "Images",
                folder);

            Directory.CreateDirectory(uploadsFolder);

            string extension =
                Path.GetExtension(file.Name).ToLowerInvariant();

            var allowed =
                new[] { ".jpg", ".jpeg", ".png", ".webp" };

            if (!allowed.Contains(extension))
                throw new InvalidOperationException("Invalid image format");

            string finalName = $"{fileName}{extension}";
            string fullPath = Path.Combine(uploadsFolder, finalName);

            await using var fs =
                new FileStream(fullPath, FileMode.Create);

            await file.OpenReadStream(maxFileSize)
                      .CopyToAsync(fs);

            return $"/Images/{folder}/{finalName}";
        }

        public Task DeleteImageAsync(string imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
                return Task.CompletedTask;

            try
            {
                string wwwRoot = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot");

                string relativePath = imagePath.TrimStart('/');

                string fullPath = Path.GetFullPath(
                    Path.Combine(wwwRoot, relativePath));

                // Proteção contra path traversal
                if (!fullPath.StartsWith(wwwRoot))
                    throw new InvalidOperationException("Invalid image path.");

                if (!File.Exists(fullPath))
                    return Task.CompletedTask;

                File.Delete(fullPath);

                _logger.LogInformation(
                "Imagem removida: {ImagePath}",
                fullPath);

                return Task.CompletedTask;
            }
            catch
            {
                
                throw;
            }
        }


        public async Task<string> ReplaceImageAsync(
            IBrowserFile file,
            string folder,
            string fileName,
            string? existingImagePath = null)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            // 1. Se já existe imagem anterior, apagar
            if (!string.IsNullOrWhiteSpace(existingImagePath))
            {
                await DeleteImageAsync(existingImagePath);
            }

            // 2. Guardar nova imagem
            string newPath = await SaveImageAsync(
                file,
                folder,
                fileName);

            _logger.LogInformation(
                "Imagem substituída em {Folder} com nome {FileName}",
                folder,
                fileName);

            return newPath;
        }

    }
}
