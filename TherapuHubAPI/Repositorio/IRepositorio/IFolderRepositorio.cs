using TherapuHubAPI.Models;

namespace TherapuHubAPI.Repositorio.IRepositorio;

public interface IFolderRepositorio : IRepository<Folders>
{
    /// <summary>Returns all active folders (system users see everything).</summary>
    Task<IEnumerable<Folders>> GetByCompanyAndTypeAsync(int companyId, byte folderTypeId, int? sectionId = null);

    /// <summary>Returns only global folders + folders created by the given actor.</summary>
    Task<IEnumerable<Folders>> GetVisibleByCompanyAndTypeAsync(int companyId, byte folderTypeId, int actorId, int? sectionId = null);

    /// <summary>Returns all direct subfolders of a parent (system users).</summary>
    Task<IEnumerable<Folders>> GetSubfoldersAsync(int parentFolderId, int companyId);

    /// <summary>Returns visible direct subfolders of a parent (global + created by actor).</summary>
    Task<IEnumerable<Folders>> GetVisibleSubfoldersAsync(int parentFolderId, int companyId, int actorId);

    Task<Folders?> GetByIdAndCompanyAsync(int id, int companyId);
    Task<bool> ExistsByNameAsync(int companyId, byte folderTypeId, string name, int? excludeId = null, int? parentFolderId = null);
}
