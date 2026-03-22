using TherapuHubAPI.DTOs.Requests.Notes;
using TherapuHubAPI.DTOs.Responses.Notes;

namespace TherapuHubAPI.Services.IServices;

public interface INotesService
{
    Task<IEnumerable<NoteResponseDto>> GetByModuleEntityAsync(int companyId, int moduleId, int? entityId);
    Task<NoteResponseDto?> GetByIdAsync(long id, int companyId);
    Task<NoteResponseDto> CreateAsync(CreateNoteRequestDto dto, int companyId, int createdByUserId);
    Task<NoteResponseDto?> UpdateAsync(long id, UpdateNoteRequestDto dto, int companyId);
    Task<bool> DeleteAsync(long id, int companyId);
    Task<bool> ToggleActiveAsync(long id, int companyId);
    Task<IEnumerable<NotePriorityResponseDto>> GetPrioritiesAsync();
}
