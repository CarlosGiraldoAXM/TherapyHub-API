using TherapuHubAPI.DTOs.Requests.Library;
using TherapuHubAPI.DTOs.Responses.Library;

namespace TherapuHubAPI.Services.IServices;

public interface ILibraryItemService
{
    Task<IEnumerable<LibraryCategoryResponseDto>> GetCategoriesAsync();
    Task<IEnumerable<LibraryItemResponseDto>> GetByCategorySlugAsync(string categorySlug);
    Task<LibraryItemResponseDto?> GetByIdAsync(int id);
    Task<LibraryItemResponseDto> CreateAsync(CreateLibraryItemRequestDto dto, int createdByActorId);
    Task<LibraryItemResponseDto?> UpdateAsync(int id, UpdateLibraryItemRequestDto dto);
    Task<bool> DeleteAsync(int id, int deletedByActorId);
    Task<LibraryItemFileResponseDto> UploadFileAsync(int itemId, IFormFile file, int uploaderActorId);
    Task<(Stream Stream, string ContentType, string FileName)?> DownloadFileAsync(int itemId, int associationId);
    Task<bool> DeleteFileAsync(int itemId, int associationId, int deletedByActorId);
}
