using TherapuHubAPI.Models;
using TherapuHubAPI.Repositorio.IRepositorio;

namespace TherapuHubAPI.Repositorio.IRepositorio;

public interface ITipoUsuarioRepositorio : IRepository<UserTypes>
{
    Task<UserTypes?> GetByNombreAsync(string nombre);
    Task<bool> ExistsByNombreAsync(string nombre, int? excludeId = null);
}
