using TherapuHubAPI.Models;
using TherapuHubAPI.Repositorio.IRepositorio;

namespace TherapuHubAPI.Repositorio.IRepositorio;

public interface IUsuarioRepositorio : IRepository<Users>
{
    Task<Users?> GetByCorreoAsync(string correo);
    new Task<Users?> GetByIdAsync(int id);
    new Task<IEnumerable<Users>> GetAllAsync();
    Task<int> CountByTipoUsuarioIdAsync(int tipoUsuarioId);
    Task<bool> HasUsersInCompanyAsync(int companyId);
}
