using TherapuHubAPI.Models;

namespace TherapuHubAPI.Repositorio.IRepositorio;

public interface IMenuRepositorio : IRepository<Menus>
{
    Task<Menus?> GetByRutaAsync(string ruta);
    Task<bool> ExistsByRutaAsync(string ruta, int? excludeId = null);
    Task<IEnumerable<Menus>> GetMenusByTipoUsuarioIdAsync(int tipoUsuarioId);
}
