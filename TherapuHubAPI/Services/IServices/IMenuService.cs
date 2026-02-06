using TherapuHubAPI.DTOs.Requests.Menus;
using TherapuHubAPI.DTOs.Responses.Menus;

namespace TherapuHubAPI.Services.IServices;

public interface IMenuService
{
    Task<IEnumerable<MenuResponseDto>> GetAllAsync();
    Task<MenuResponseDto?> GetByIdAsync(int id);
    Task<IEnumerable<MenuResponseDto>> GetMenusByTipoUsuarioIdAsync(int tipoUsuarioId);
    Task<TipoUsuarioConMenusResponseDto> GetTipoUsuarioConMenusAsync(int tipoUsuarioId);
    Task AsignarMenusAsync(AsignarMenusRequestDto request);
    Task<IEnumerable<MenuResponseDto>> GetMenusUsuarioActualAsync(int tipoUsuarioId);
    Task<MenuResponseDto> CreateAsync(CreateMenuRequestDto request);
    Task<MenuResponseDto> UpdateAsync(int id, UpdateMenuRequestDto request);
    Task DeleteAsync(int id);
    Task MoveUpAsync(int id);
    Task MoveDownAsync(int id);
}
