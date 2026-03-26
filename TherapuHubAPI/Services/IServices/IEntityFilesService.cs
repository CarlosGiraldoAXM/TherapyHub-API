using TherapuHubAPI.DTOs.Responses.Files;

namespace TherapuHubAPI.Services.IServices;

public interface IEntityFilesService
{
    Task<IEnumerable<EntityFileResponseDto>> GetByOwnerAndTypeAsync(int ownerActorId, int filesTypeId);
    Task<EntityFileResponseDto> UploadAsync(IFormFile file, int ownerActorId, int filesTypeId, int uploaderActorId, int companyId);
    Task<(Stream Stream, string ContentType, string FileName)?> DownloadAsync(long fileId);
    Task<bool> DeleteAsync(long fileId, int deleterActorId);
}
