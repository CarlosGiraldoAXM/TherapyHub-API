using Microsoft.EntityFrameworkCore;
using TherapuHubAPI.Models;
using TherapuHubAPI.Repositorio.IRepositorio;

namespace TherapuHubAPI.Repositorio;

public class FileRepositorio : Repository<Files>, IFileRepositorio
{
    public FileRepositorio(ContextDB context) : base(context)
    {
    }

    public async Task<Files?> GetByIdLongAsync(long id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<Files>> GetByFolderAsync(int folderId)
    {
        return await _dbSet
            .Where(f => f.FolderId == folderId && !f.IsDeleted)
            .OrderBy(f => f.FileName)
            .ToListAsync();
    }

    public async Task<Files?> GetByIdAndFolderAsync(long id, int folderId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(f => f.Id == id && f.FolderId == folderId && !f.IsDeleted);
    }

    public async Task<int> CountByFolderAsync(int folderId)
    {
        return await _dbSet.CountAsync(f => f.FolderId == folderId && !f.IsDeleted);
    }

    public async Task<IEnumerable<Files>> GetByOwnerAndTypeAsync(int ownerActorId, int filesTypeId)
    {
        return await _dbSet
            .Where(f => f.OwnerActorId == ownerActorId && f.FilesTypeId == filesTypeId && !f.IsDeleted)
            .OrderByDescending(f => f.UploadedAt)
            .ToListAsync();
    }

    public async Task<Files?> GetByIdAsync(long id)
    {
        return await _dbSet.FirstOrDefaultAsync(f => f.Id == id && !f.IsDeleted);
    }
}
