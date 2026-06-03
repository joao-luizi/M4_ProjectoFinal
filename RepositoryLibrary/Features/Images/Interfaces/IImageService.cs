using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Features.Images.Interfaces
{
    public interface IImageService
    {
        Task<string> SaveImageAsync(
            IBrowserFile file,
            string folder,
            string fileName);

        Task DeleteImageAsync(string imagePath);

        Task<string> ReplaceImageAsync(
            IBrowserFile file,
            string folder,
            string fileName,
            string? existingImagePath = null);
    }
}
