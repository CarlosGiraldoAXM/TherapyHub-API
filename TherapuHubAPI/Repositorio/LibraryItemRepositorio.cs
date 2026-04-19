using Microsoft.EntityFrameworkCore;
using TherapuHubAPI.Models;
using TherapuHubAPI.Repositorio.IRepositorio;

namespace TherapuHubAPI.Repositorio;

public class LibraryItemRepositorio : Repository<LibraryItems>, ILibraryItemRepositorio
{
    private readonly ContextDB _db;

    public LibraryItemRepositorio(ContextDB context) : base(context)
    {
        _db = context;
    }

    public async Task<IEnumerable<LibraryItems>> GetByCategoryAsync(byte categoryId)
    {
        return await _db.LibraryItems
            .Where(x => x.CategoryId == categoryId && !x.IsDeleted)
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<LibraryItems?> GetByIdWithFilesAsync(int id)
    {
        return await _db.LibraryItems
            .Include(x => x.Category)
            .Include(x => x.LibraryItemFiles.Where(f => !f.IsDeleted))
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
    }

    public async Task<IEnumerable<LibraryCategories>> GetAllCategoriesAsync()
    {
        return await _db.LibraryCategories
            .Where(c => c.IsActive)
            .OrderBy(c => c.Id)
            .ToListAsync();
    }

    public async Task AddLibraryItemFileAsync(LibraryItemFiles entry)
    {
        await _db.LibraryItemFiles.AddAsync(entry);
    }

    public void UpdateLibraryItemFile(LibraryItemFiles entry)
    {
        _db.LibraryItemFiles.Update(entry);
    }
}
