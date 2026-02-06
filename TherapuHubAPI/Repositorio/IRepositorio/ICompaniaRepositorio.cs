using TherapuHubAPI.Models;

namespace TherapuHubAPI.Repositorio.IRepositorio;

public interface ICompaniaRepositorio : IRepository<Companies>
{
    Task<Companies?> GetByIdCompaniaAsync(int idCompania);
    Task<Companies?> GetByNombreAsync(string nombre);
}
