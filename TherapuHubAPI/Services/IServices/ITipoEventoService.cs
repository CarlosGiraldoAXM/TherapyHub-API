using TherapuHubAPI.DTOs.Responses.Events;

namespace TherapuHubAPI.Services.IServices;

public interface ITipoEventoService
{
    Task<IEnumerable<TipoEventoResponseDto>> GetTiposEventoActivosAsync();
}
