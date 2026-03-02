using Microsoft.EntityFrameworkCore;
using TherapuHubAPI.Models;
using TherapuHubAPI.Repositorio.IRepositorio;

namespace TherapuHubAPI.Repositorio;

public class CompaniaRepositorio : Repository<Companies>, ICompaniaRepositorio
{
    public CompaniaRepositorio(ContextDB context) : base(context)
    {
    }

    public override async Task<IEnumerable<Companies>> GetAllAsync()
    {
        return await _dbSet
            .Where(c => !c.IsDeleted)
            .ToListAsync();
    }

    public async Task<Companies?> GetByIdCompaniaAsync(int idCompania)
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => c.Id == idCompania && !c.IsDeleted);
    }

    public async Task<Companies?> GetByNombreAsync(string nombre)
    {
        var nombreNormalizado = nombre.Trim().ToLower();
        return await _dbSet
            .FirstOrDefaultAsync(c => c.Name.Trim().ToLower() == nombreNormalizado && !c.IsDeleted);
    }
}
