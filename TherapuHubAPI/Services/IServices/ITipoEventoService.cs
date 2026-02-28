using TherapuHubAPI.DTOs.Requests.Events;
using TherapuHubAPI.DTOs.Responses.Events;

namespace TherapuHubAPI.Services.IServices;

public interface ITipoEventoService
{
    Task<IEnumerable<TipoEventoResponseDto>> GetTiposEventoActivosAsync();
    Task<IEnumerable<TipoEventoResponseDto>> GetAllAsync();
    Task<TipoEventoResponseDto?> GetByIdAsync(int id);
    Task<TipoEventoResponseDto> CreateAsync(CreateEventTypeRequestDto dto);
    Task<TipoEventoResponseDto?> UpdateAsync(int id, UpdateEventTypeRequestDto dto);
    Task<bool> DeleteAsync(int id);
    Task<TipoEventoResponseDto?> ToggleActiveAsync(int id);
}
