using TherapuHubAPI.DTOs.Requests.SessionNotesStatus;
using TherapuHubAPI.DTOs.Responses.SessionNotesStatus;

namespace TherapuHubAPI.Services.IServices;

public interface ISessionNotesStatusService
{
    Task<IEnumerable<SessionNotesStatusResponseDto>> GetAllAsync();
    Task<IEnumerable<SessionNotesStatusResponseDto>> GetActiveAsync();
    Task<SessionNotesStatusResponseDto?> GetByIdAsync(int id);
    Task<SessionNotesStatusResponseDto> CreateAsync(CreateSessionNotesStatusRequestDto dto, int actorId);
    Task<SessionNotesStatusResponseDto?> UpdateAsync(int id, UpdateSessionNotesStatusRequestDto dto);
    Task<bool> DeleteAsync(int id, int actorId);
    Task<SessionNotesStatusResponseDto?> ToggleActiveAsync(int id);
}
