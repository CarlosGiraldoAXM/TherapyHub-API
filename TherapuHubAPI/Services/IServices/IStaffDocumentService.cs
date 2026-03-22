using TherapuHubAPI.DTOs.Responses.Staff;

namespace TherapuHubAPI.Services.IServices;

public interface IStaffDocumentService
{
    Task<IEnumerable<StaffDocumentResponseDto>> GetDocumentsByStaffAsync(int staffId, int companyId);
    Task<StaffDocumentResponseDto> UploadDocumentAsync(int staffId, int companyId, IFormFile file, byte documentTypeId);
    Task<(Stream Stream, string ContentType, string FileName)> DownloadDocumentAsync(long documentId, int staffId, int companyId);
    Task DeleteDocumentAsync(long documentId, int staffId, int companyId);
}
