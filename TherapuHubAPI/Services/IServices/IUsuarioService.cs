using TherapuHubAPI.DTOs.Requests.Auth;
using TherapuHubAPI.DTOs.Requests.Users;
using TherapuHubAPI.DTOs.Responses.Users;

namespace TherapuHubAPI.Services.IServices;

public interface IUsuarioService
{
    Task<UsuarioResponseDto> CreateAsync(CreateUsuarioRequestDto request, int currentUserId);
    Task<UsuarioResponseDto?> GetByIdAsync(int id);
    Task<IEnumerable<UsuarioResponseDto>> GetAllAsync(int currentUserId);
    Task<UsuarioResponseDto?> UpdateAsync(int id, UpdateUsuarioRequestDto request);
    Task<bool> DeleteAsync(int id, int deleteUserId);
    Task<bool> ToggleActivoAsync(int id);
    Task SetInitialPasswordAsync(int userId, SetInitialPasswordRequestDto request);
    Task ResetPasswordAsync(int userId);
}
