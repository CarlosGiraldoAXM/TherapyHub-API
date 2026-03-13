using TherapuHubAPI.Models;

namespace TherapuHubAPI.Repositorio.IRepositorio;

public interface IFileRepositorio : IRepository<Files>
{
    Task<Files?> GetByIdLongAsync(long id);
    Task<IEnumerable<Files>> GetByFolderAsync(int folderId);
    Task<Files?> GetByIdAndFolderAsync(long id, int folderId);
    Task<int> CountByFolderAsync(int folderId);
}
