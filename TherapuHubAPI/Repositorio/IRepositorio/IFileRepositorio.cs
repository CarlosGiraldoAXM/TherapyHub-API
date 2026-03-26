using TherapuHubAPI.Models;

namespace TherapuHubAPI.Repositorio.IRepositorio;

public interface IFileRepositorio : IRepository<Files>
{
    Task<Files?> GetByIdLongAsync(long id);
    Task<IEnumerable<Files>> GetByFolderAsync(int folderId);
    Task<Files?> GetByIdAndFolderAsync(long id, int folderId);
    Task<int> CountByFolderAsync(int folderId);
    Task<IEnumerable<Files>> GetByOwnerAndTypeAsync(int ownerActorId, int filesTypeId);
    Task<Files?> GetByIdAsync(long id);
}
