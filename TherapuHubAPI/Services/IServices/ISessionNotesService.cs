using TherapuHubAPI.DTOs.Requests.SessionNotes;
using TherapuHubAPI.DTOs.Responses.SessionNotes;

namespace TherapuHubAPI.Services.IServices;

public interface ISessionNotesService
{
    Task<IEnumerable<RbtForSessionResponseDto>> GetRbtsForUserAsync(int userId, int companyId);
    Task<IEnumerable<SessionNoteStatusResponseDto>> GetStatusesAsync();
    Task<IEnumerable<SessionNoteResponseDto>> GetByWeekAsync(int companyId, DateTime weekStart, DateTime weekEnd);
    Task<SessionNoteResponseDto> CreateAsync(CreateSessionNoteRequestDto dto, int actorId);
    Task<SessionNoteResponseDto?> UpdateAsync(long id, UpdateSessionNoteRequestDto dto, int actorId);
    Task<bool> DeleteAsync(long id, int actorId);
}
