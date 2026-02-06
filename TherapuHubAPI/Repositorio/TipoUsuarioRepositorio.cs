using Microsoft.EntityFrameworkCore;
using TherapuHubAPI.Models;
using TherapuHubAPI.Repositorio.IRepositorio;

namespace TherapuHubAPI.Repositorio;

public class TipoUsuarioRepositorio : Repository<UserTypes>, ITipoUsuarioRepositorio
{
    public TipoUsuarioRepositorio(ContextDB context) : base(context)
    {
    }

    public async Task<UserTypes?> GetByNombreAsync(string nombre)
    {
        return await _dbSet
            .FirstOrDefaultAsync(t => t.Name.ToLower() == nombre.ToLower());
    }

    public async Task<bool> ExistsByNombreAsync(string nombre, int? excludeId = null)
    {
        var query = _dbSet.Where(t => t.Name.ToLower() == nombre.ToLower());
        
        if (excludeId.HasValue)
        {
            query = query.Where(t => t.Id != excludeId.Value);
        }
        
        return await query.AnyAsync();
    }
}
