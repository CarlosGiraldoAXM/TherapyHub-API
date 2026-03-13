using TherapuHubAPI.DTOs.Requests.Folders;
using TherapuHubAPI.DTOs.Responses.Folders;

namespace TherapuHubAPI.Services.IServices;

public interface IFolderService
{
    // Folders
    Task<IEnumerable<FolderResponseDto>> GetFoldersByTypeAsync(int companyId, byte folderTypeId, int userId, int userTypeId);
    Task<IEnumerable<FolderResponseDto>> GetSubfoldersAsync(int parentFolderId, int companyId, int userId, int userTypeId);
    Task<FolderResponseDto?> GetFolderByIdAsync(int id, int companyId);
    Task<FolderResponseDto> CreateFolderAsync(CreateFolderRequestDto request, int companyId, int userId, int userTypeId);
    Task<FolderResponseDto?> UpdateFolderAsync(int id, UpdateFolderRequestDto request, int companyId);
    /// <returns>true = deleted, false = not found, throws UnauthorizedAccessException if caller is not the owner</returns>
    Task<bool> DeleteFolderAsync(int id, int companyId, int userId, int userTypeId);

    // Files
    Task<IEnumerable<FileResponseDto>> GetFilesAsync(int folderId, int companyId);
    Task<FileResponseDto> UploadFileAsync(int folderId, IFormFile file, int companyId, int userId);
    Task<(Stream Stream, string ContentType, string FileName)?> DownloadFileAsync(long fileId, int folderId, int companyId);
    Task<bool> DeleteFileAsync(long fileId, int folderId, int companyId);
}
