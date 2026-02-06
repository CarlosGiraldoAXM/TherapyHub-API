using TherapuHubAPI.DTOs.Requests.Events;
using TherapuHubAPI.DTOs.Responses.Events;

namespace TherapuHubAPI.Services.IServices;

public interface IEventosService
{
    Task<IEnumerable<EventoResponseDto>> GetEventosByUserAsync(int userId, DateTime? start, DateTime? end, bool? esTodoElDia = null);
    Task<EventoResponseDto?> GetEventoByIdAsync(int id, int currentUserId);
    Task<EventoResponseDto> CreateEventoAsync(CreateEventoRequestDto request, int currentUserId);
    Task<bool> UpdateEventoAsync(int id, UpdateEventoRequestDto request, int currentUserId);
    Task<bool> DeleteEventoAsync(int id, int currentUserId);
}
