using Microsoft.AspNetCore.Hosting;

namespace Server.Services;

public class FileUploadService
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<FileUploadService> _logger;
    private readonly string _uploadsPath;

    public FileUploadService(IWebHostEnvironment environment, ILogger<FileUploadService> logger)
    {
        _environment = environment;
        _logger = logger;
        _uploadsPath = Path.Combine(_environment.WebRootPath, "uploads", "avatars");
        
        // Tạo thư mục nếu chưa tồn tại
        if (!Directory.Exists(_uploadsPath))
        {
            Directory.CreateDirectory(_uploadsPath);
        }
    }

    public async Task<string?> UploadAvatarAsync(IFormFile file, Guid userId)
    {
        if (file == null || file.Length == 0)
        {
            return null;
        }

        // Validate file type
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        
        if (!allowedExtensions.Contains(extension))
        {
            _logger.LogWarning("Invalid file type: {Extension}", extension);
            throw new ArgumentException("Chỉ chấp nhận file ảnh: JPG, PNG, GIF, WEBP");
        }

        // Validate file size (max 5MB)
        const long maxFileSize = 5 * 1024 * 1024; // 5MB
        if (file.Length > maxFileSize)
        {
            _logger.LogWarning("File too large: {Size} bytes", file.Length);
            throw new ArgumentException("File không được vượt quá 5MB");
        }

        try
        {
            // Generate unique filename
            var fileName = $"{userId}_{DateTime.UtcNow:yyyyMMddHHmmss}{extension}";
            var filePath = Path.Combine(_uploadsPath, fileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return relative URL
            var url = $"/uploads/avatars/{fileName}";
            _logger.LogInformation("Avatar uploaded: {Url}", url);
            
            return url;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading avatar");
            throw;
        }
    }

    public bool DeleteAvatar(string? avatarUrl)
    {
        if (string.IsNullOrEmpty(avatarUrl))
        {
            return false;
        }

        try
        {
            // Chỉ xóa file local, không xóa URL external
            if (avatarUrl.StartsWith("/uploads/avatars/"))
            {
                var fileName = Path.GetFileName(avatarUrl);
                var filePath = Path.Combine(_uploadsPath, fileName);
                
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    _logger.LogInformation("Deleted avatar: {Path}", filePath);
                    return true;
                }
            }
            
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting avatar");
            return false;
        }
    }
}
