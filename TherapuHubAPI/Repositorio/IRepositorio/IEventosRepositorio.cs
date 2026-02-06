using TherapuHubAPI.Models;

namespace TherapuHubAPI.Repositorio.IRepositorio;

public interface IEventosRepositorio : IRepository<Events>
{
    Task<IEnumerable<Events>> GetEventosByUserAsync(int userId, int userCompaniaId, DateTime? start, DateTime? end, bool? esTodoElDia = null);
}
