namespace hotel_room_api.Controllers;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

public class ImageHandler
{
    
    public static async Task<(string ImageUrl, string ImageLocalPath)> UpdateImageAsync(
        IFormFile newImage,
        string existingImageLocalPath,
        string wwwRootPath,
        string scheme,
        string host,
        int entityId)
    {
        
        await DeleteImageAsync(existingImageLocalPath, wwwRootPath);
        
        return await SaveNewImageAsync(newImage, wwwRootPath, scheme, host, entityId);
    }

    
    public static async Task DeleteImageAsync(string imageLocalPath, string wwwRootPath)
    {
        if (!string.IsNullOrEmpty(imageLocalPath))
        {
            string fullPath = Path.Combine(wwwRootPath, imageLocalPath.TrimStart('/'));
            if (File.Exists(fullPath))
            {
                await Task.Run(() => File.Delete(fullPath));
            }
        }
    }
    
    public static async Task<(string ImageUrl, string ImageLocalPath)> SaveNewImageAsync(
        IFormFile image,
        string wwwRootPath,
        string scheme,
        string host,
        int entityId)
    {
        if (!IsValidImage(image))
            return ("", "");
            //throw new ArgumentException("Invalid image format");

        string fileExtension = Path.GetExtension(image.FileName)?.ToLowerInvariant();
        string fileName = $"{Guid.NewGuid()}_{entityId}{fileExtension}";
        string imagesFolder = Path.Combine(wwwRootPath, "images");
        Directory.CreateDirectory(imagesFolder);

        string filePath = Path.Combine(imagesFolder, fileName);

        await using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            await image.CopyToAsync(fileStream);
        }

        var baseUrl = $"{scheme}://{host}";
        string imageUrl = $"{baseUrl}/images/{fileName}";
        string imageLocalPath = $"/images/{fileName}";

        return (imageUrl, imageLocalPath);
    }
    
    private static bool IsValidImage(IFormFile file)
    {
        if (file == null || file.Length > 5 * 1024 * 1024) // 5MB limit
            return false;

        string[] permittedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        var ext = Path.GetExtension(file.FileName)?.ToLowerInvariant();
        return !string.IsNullOrEmpty(ext) && permittedExtensions.Contains(ext);
    }
}