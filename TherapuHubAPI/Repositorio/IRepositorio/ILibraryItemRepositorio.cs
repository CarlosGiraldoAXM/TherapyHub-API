using TherapuHubAPI.Models;

namespace TherapuHubAPI.Repositorio.IRepositorio;

public interface ILibraryItemRepositorio : IRepository<LibraryItems>
{
    Task<IEnumerable<LibraryItems>> GetByCategoryAsync(byte categoryId);
    Task<LibraryItems?> GetByIdWithFilesAsync(int id);
    Task<IEnumerable<LibraryCategories>> GetAllCategoriesAsync();
    Task AddLibraryItemFileAsync(LibraryItemFiles entry);
    void UpdateLibraryItemFile(LibraryItemFiles entry);
}
