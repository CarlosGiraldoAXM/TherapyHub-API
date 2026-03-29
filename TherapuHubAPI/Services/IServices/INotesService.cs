using TherapuHubAPI.DTOs.Requests.Notes;
using TherapuHubAPI.DTOs.Responses.Notes;

namespace TherapuHubAPI.Services.IServices;

public interface INotesService
{
    Task<IEnumerable<NoteResponseDto>> GetByOwnerAsync(int companyId, int menuId, int ownerActorId, int? sectionId = null);
    Task<NoteResponseDto?> GetByIdAsync(long id, int companyId);
    Task<NoteResponseDto> CreateAsync(CreateNoteRequestDto dto, int companyId, int createdByActorId);
    Task<NoteResponseDto?> UpdateAsync(long id, UpdateNoteRequestDto dto, int companyId);
    Task<bool> DeleteAsync(long id, int companyId);
    Task<bool> ToggleActiveAsync(long id, int companyId);
    Task<IEnumerable<NotePriorityResponseDto>> GetPrioritiesAsync();
}
