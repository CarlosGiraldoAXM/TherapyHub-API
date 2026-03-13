namespace TherapuHubAPI.Services.IServices;

public interface IFileStorageService
{
    /// <summary>
    /// Saves a file and returns the blob path (relative path used as BlobPath in DB).
    /// </summary>
    Task<string> SaveFileAsync(IFormFile file, string containerPath);

    /// <summary>
    /// Retrieves a file stream, content type, and original filename from its blob path.
    /// </summary>
    Task<(Stream Stream, string ContentType, string FileName)> GetFileAsync(string blobPath);

    /// <summary>
    /// Returns the byte size of a stored file.
    /// </summary>
    Task<long> GetFileSizeAsync(string blobPath);

    /// <summary>
    /// Deletes a file from storage.
    /// </summary>
    Task DeleteFileAsync(string blobPath);

    /// <summary>
    /// Resolves the MIME content type for a given file extension.
    /// </summary>
    string ResolveContentType(string fileName);
}
