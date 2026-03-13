using TherapuHubAPI.Services.IServices;

namespace TherapuHubAPI.Services.Implementations;

/// <summary>
/// Local disk-based file storage. Used in development.
/// Replace with an Azure Blob Storage implementation for production.
/// Files are stored under: {ContentRoot}/uploads/{containerPath}
/// </summary>
public class LocalFileStorageService : IFileStorageService
{
    private readonly string _basePath;

    public LocalFileStorageService(IWebHostEnvironment env)
    {
        _basePath = Path.Combine(env.ContentRootPath, "uploads");
        Directory.CreateDirectory(_basePath);
    }

    public async Task<string> SaveFileAsync(IFormFile file, string containerPath)
    {
        var containerDir = Path.Combine(_basePath, containerPath);
        Directory.CreateDirectory(containerDir);

        // Sanitize filename and avoid collisions
        var safeName = Path.GetFileNameWithoutExtension(file.FileName);
        var ext = Path.GetExtension(file.FileName);
        var uniqueName = $"{safeName}_{Guid.NewGuid():N}{ext}";

        var fullPath = Path.Combine(containerDir, uniqueName);

        await using var stream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(stream);

        // Return a forward-slash path usable as BlobPath
        return $"{containerPath}/{uniqueName}".Replace("\\", "/");
    }

    public Task<(Stream Stream, string ContentType, string FileName)> GetFileAsync(string blobPath)
    {
        var fullPath = GetFullPath(blobPath);

        if (!File.Exists(fullPath))
            throw new FileNotFoundException($"File not found: {blobPath}");

        var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        var fileName = Path.GetFileName(fullPath);
        var contentType = ResolveContentType(fileName);

        return Task.FromResult<(Stream, string, string)>((stream, contentType, fileName));
    }

    public Task<long> GetFileSizeAsync(string blobPath)
    {
        var fullPath = GetFullPath(blobPath);
        return Task.FromResult(File.Exists(fullPath) ? new FileInfo(fullPath).Length : 0L);
    }

    public Task DeleteFileAsync(string blobPath)
    {
        var fullPath = GetFullPath(blobPath);
        if (File.Exists(fullPath))
            File.Delete(fullPath);
        return Task.CompletedTask;
    }

    public string ResolveContentType(string fileName)
    {
        return Path.GetExtension(fileName).ToLowerInvariant() switch
        {
            ".pdf"  => "application/pdf",
            ".jpg"  => "image/jpeg",
            ".jpeg" => "image/jpeg",
            ".png"  => "image/png",
            ".gif"  => "image/gif",
            ".webp" => "image/webp",
            ".doc"  => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls"  => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".ppt"  => "application/vnd.ms-powerpoint",
            ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
            ".txt"  => "text/plain",
            ".csv"  => "text/csv",
            ".zip"  => "application/zip",
            _       => "application/octet-stream"
        };
    }

    private string GetFullPath(string blobPath) =>
        Path.Combine(_basePath, blobPath.Replace("/", Path.DirectorySeparatorChar.ToString()));
}
