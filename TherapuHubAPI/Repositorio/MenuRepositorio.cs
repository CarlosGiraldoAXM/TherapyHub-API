using Microsoft.EntityFrameworkCore;
using TherapuHubAPI.Models;
using TherapuHubAPI.Repositorio.IRepositorio;

namespace TherapuHubAPI.Repositorio;

public class MenuRepositorio : Repository<Menus>, IMenuRepositorio
{
    private new readonly ContextDB _context;

    public MenuRepositorio(ContextDB context) : base(context)
    {
        _context = context;
    }

    public async Task<Menus?> GetByRutaAsync(string ruta)
    {
        return await _context.Menus
            .FirstOrDefaultAsync(m => m.Route == ruta);
    }

    public async Task<bool> ExistsByRutaAsync(string ruta, int? excludeId = null)
    {
        var query = _context.Menus.Where(m => m.Route == ruta);
        
        if (excludeId.HasValue)
        {
            query = query.Where(m => m.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<IEnumerable<Menus>> GetMenusByTipoUsuarioIdAsync(int tipoUsuarioId)
    {
        return await _context.Menus
            .Where(m => m.IsActive && _context.UserTypeMenus.Any(tum => tum.UserTypeId == tipoUsuarioId && tum.MenuId == m.Id))
            .OrderBy(m => m.SortOrder)
            .ToListAsync();
    }
}
