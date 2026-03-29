using Microsoft.EntityFrameworkCore;
using TherapuHubAPI.Models;
using TherapuHubAPI.Repositorio.IRepositorio;

namespace TherapuHubAPI.Repositorio;

public class FolderRepositorio : Repository<Folders>, IFolderRepositorio
{
    public FolderRepositorio(ContextDB context) : base(context)
    {
    }

    public async Task<IEnumerable<Folders>> GetByCompanyAndTypeAsync(int companyId, byte folderTypeId, int? sectionId = null)
    {
        return await _dbSet
            .Where(f => f.CompanyId == companyId && f.FolderTypeId == folderTypeId && f.ParentFolderId == null && !f.IsDeleted && f.IsActive
                        && (sectionId == null ? f.SectionId == null : f.SectionId == sectionId))
            .OrderBy(f => f.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Folders>> GetVisibleByCompanyAndTypeAsync(int companyId, byte folderTypeId, int actorId, int? sectionId = null)
    {
        return await _dbSet
            .Where(f =>
                f.CompanyId == companyId &&
                f.FolderTypeId == folderTypeId &&
                f.ParentFolderId == null &&
                !f.IsDeleted &&
                f.IsActive &&
                (f.IsGlobal || f.CreatedByActorId == actorId) &&
                (sectionId == null ? f.SectionId == null : f.SectionId == sectionId))
            .OrderBy(f => f.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Folders>> GetSubfoldersAsync(int parentFolderId, int companyId)
    {
        return await _dbSet
            .Where(f => f.ParentFolderId == parentFolderId && f.CompanyId == companyId && !f.IsDeleted && f.IsActive)
            .OrderBy(f => f.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Folders>> GetVisibleSubfoldersAsync(int parentFolderId, int companyId, int actorId)
    {
        return await _dbSet
            .Where(f =>
                f.ParentFolderId == parentFolderId &&
                f.CompanyId == companyId &&
                !f.IsDeleted &&
                f.IsActive &&
                (f.IsGlobal || f.CreatedByActorId == actorId))
            .OrderBy(f => f.Name)
            .ToListAsync();
    }

    public async Task<Folders?> GetByIdAndCompanyAsync(int id, int companyId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(f => f.Id == id && f.CompanyId == companyId && !f.IsDeleted);
    }

    public async Task<bool> ExistsByNameAsync(int companyId, byte folderTypeId, string name, int? excludeId = null, int? parentFolderId = null)
    {
        return await _dbSet.AnyAsync(f =>
            f.CompanyId == companyId &&
            f.FolderTypeId == folderTypeId &&
            f.ParentFolderId == parentFolderId &&
            !f.IsDeleted &&
            f.Name.ToLower() == name.ToLower() &&
            (excludeId == null || f.Id != excludeId));
    }
}
