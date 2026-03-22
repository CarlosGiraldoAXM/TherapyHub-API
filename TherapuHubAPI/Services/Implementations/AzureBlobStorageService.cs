using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using TherapuHubAPI.Services.IServices;

namespace TherapuHubAPI.Services.Implementations;

/// <summary>
/// Azure Blob Storage implementation of IFileStorageService.
/// Files are stored in a single container using {containerPath}/{uniqueName} as the blob name.
/// </summary>
public class AzureBlobStorageService : IFileStorageService
{
    private readonly BlobContainerClient _container;

    public AzureBlobStorageService(IConfiguration configuration)
    {
        var connectionString = configuration["AzureBlobStorage:ConnectionString"]
            ?? throw new InvalidOperationException("AzureBlobStorage:ConnectionString is not configured.");

        var containerName = configuration["AzureBlobStorage:ContainerName"]
            ?? throw new InvalidOperationException("AzureBlobStorage:ContainerName is not configured.");

        var serviceClient = new BlobServiceClient(connectionString);
        _container = serviceClient.GetBlobContainerClient(containerName);

        // Ensure the container exists (public access off — blobs are private)
        _container.CreateIfNotExists(PublicAccessType.None);
    }

    public async Task<string> SaveFileAsync(IFormFile file, string containerPath)
    {
        var safeName = Path.GetFileNameWithoutExtension(file.FileName);
        var ext = Path.GetExtension(file.FileName);
        var uniqueName = $"{safeName}_{Guid.NewGuid():N}{ext}";
        var blobName = $"{containerPath}/{uniqueName}".Replace("\\", "/");

        var blobClient = _container.GetBlobClient(blobName);

        var uploadOptions = new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = ResolveContentType(file.FileName)
            }
        };

        await using var stream = file.OpenReadStream();
        await blobClient.UploadAsync(stream, uploadOptions);

        return blobName;
    }

    public async Task<(Stream Stream, string ContentType, string FileName)> GetFileAsync(string blobPath)
    {
        var blobClient = _container.GetBlobClient(blobPath);

        if (!await blobClient.ExistsAsync())
            throw new FileNotFoundException($"Blob not found: {blobPath}");

        var response = await blobClient.DownloadStreamingAsync();
        var contentType = response.Value.Details.ContentType ?? ResolveContentType(blobPath);
        var fileName = Path.GetFileName(blobPath);

        return (response.Value.Content, contentType, fileName);
    }

    public async Task<long> GetFileSizeAsync(string blobPath)
    {
        var blobClient = _container.GetBlobClient(blobPath);

        if (!await blobClient.ExistsAsync())
            return 0L;

        var properties = await blobClient.GetPropertiesAsync();
        return properties.Value.ContentLength;
    }

    public async Task DeleteFileAsync(string blobPath)
    {
        var blobClient = _container.GetBlobClient(blobPath);
        await blobClient.DeleteIfExistsAsync();
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
}
