using TherapuHubAPI.DTOs.Responses.Staff;
using TherapuHubAPI.Services.IServices;

namespace TherapuHubAPI.Services;

// StaffDocuments model was removed from schema. This service is temporarily stubbed.
public class StaffDocumentService : IStaffDocumentService
{
    public Task<IEnumerable<StaffDocumentResponseDto>> GetDocumentsByStaffAsync(int staffId, int companyId)
        => Task.FromResult(Enumerable.Empty<StaffDocumentResponseDto>());

    public Task<StaffDocumentResponseDto> UploadDocumentAsync(int staffId, int companyId, IFormFile file, byte documentTypeId)
        => throw new NotImplementedException("StaffDocuments feature not yet implemented in new schema.");

    public Task<(Stream Stream, string ContentType, string FileName)> DownloadDocumentAsync(long documentId, int staffId, int companyId)
        => throw new NotImplementedException("StaffDocuments feature not yet implemented in new schema.");

    public Task DeleteDocumentAsync(long documentId, int staffId, int companyId)
        => throw new NotImplementedException("StaffDocuments feature not yet implemented in new schema.");
}
