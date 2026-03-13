using TherapuHubAPI.Models;

namespace TherapuHubAPI.Repositorio.IRepositorio;

public interface IFolderRepositorio : IRepository<Folders>
{
    /// <summary>Returns all active folders (system users see everything).</summary>
    Task<IEnumerable<Folders>> GetByCompanyAndTypeAsync(int companyId, byte folderTypeId);

    /// <summary>Returns only global folders + folders owned by the given user.</summary>
    Task<IEnumerable<Folders>> GetVisibleByCompanyAndTypeAsync(int companyId, byte folderTypeId, int userId);

    /// <summary>Returns all direct subfolders of a parent (system users).</summary>
    Task<IEnumerable<Folders>> GetSubfoldersAsync(int parentFolderId, int companyId);

    /// <summary>Returns visible direct subfolders of a parent (global + owned by user).</summary>
    Task<IEnumerable<Folders>> GetVisibleSubfoldersAsync(int parentFolderId, int companyId, int userId);

    Task<Folders?> GetByIdAndCompanyAsync(int id, int companyId);
    Task<bool> ExistsByNameAsync(int companyId, byte folderTypeId, string name, int? excludeId = null, int? parentFolderId = null);
}
