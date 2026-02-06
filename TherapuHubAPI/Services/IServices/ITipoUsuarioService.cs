using TherapuHubAPI.DTOs.Requests.UserTypes;
using TherapuHubAPI.DTOs.Responses.UserTypes;

namespace TherapuHubAPI.Services.IServices;

public interface ITipoUsuarioService
{
    Task<IEnumerable<TipoUsuarioResponseDto>> GetAllAsync();
    Task<TipoUsuarioResponseDto?> GetByIdAsync(int id);
    Task<TipoUsuarioResponseDto> CreateAsync(CreateTipoUsuarioRequestDto request);
    Task<TipoUsuarioResponseDto?> UpdateAsync(int id, UpdateTipoUsuarioRequestDto request);
    Task<bool> DeleteAsync(int id);
}
