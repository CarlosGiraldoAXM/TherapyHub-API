using TherapuHubAPI.DTOs.Requests.NoteCategories;
using TherapuHubAPI.DTOs.Responses.NoteCategories;

namespace TherapuHubAPI.Services.IServices;

public interface INoteCategoryService
{
    Task<IEnumerable<NoteCategoryResponseDto>> GetBySectionAsync(int companyId, int sectionId);
    Task<NoteCategoryResponseDto?> GetByIdAsync(int id, int companyId);
    Task<NoteCategoryResponseDto> CreateAsync(CreateNoteCategoryRequestDto dto, int companyId, int actorId);
    Task<NoteCategoryResponseDto?> UpdateAsync(int id, UpdateNoteCategoryRequestDto dto, int companyId);
    Task<bool> DeleteAsync(int id, int companyId);
    Task<NoteCategoryResponseDto?> ToggleActiveAsync(int id, int companyId);
}
