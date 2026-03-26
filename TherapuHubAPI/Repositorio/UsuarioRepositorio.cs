using Microsoft.EntityFrameworkCore;
using TherapuHubAPI.Models;
using TherapuHubAPI.Repositorio.IRepositorio;

namespace TherapuHubAPI.Repositorio;

public class UsuarioRepositorio : Repository<Users>, IUsuarioRepositorio
{
    public UsuarioRepositorio(ContextDB context) : base(context)
    {
    }

    public async Task<Users?> GetByCorreoAsync(string correo)
    {
        return await _dbSet
            .Include(u => u.Actor)
            .FirstOrDefaultAsync(u => u.Actor.Email != null
                && u.Actor.Email.ToLower() == correo.ToLower()
                && !u.Actor.IsDeleted);
    }

    public new async Task<Users?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(u => u.Actor)
            .FirstOrDefaultAsync(u => u.Id == id && !u.Actor.IsDeleted);
    }

    public new async Task<IEnumerable<Users>> GetAllAsync()
    {
        return await _dbSet
            .Include(u => u.Actor)
            .Where(u => !u.Actor.IsDeleted)
            .ToListAsync();
    }

    public async Task<int> CountByTipoUsuarioIdAsync(int tipoUsuarioId)
    {
        return await _dbSet
            .Include(u => u.Actor)
            .CountAsync(u => u.UserTypeId == tipoUsuarioId && !u.Actor.IsDeleted);
    }

    public async Task<bool> HasUsersInCompanyAsync(int companyId)
    {
        return await _dbSet
            .Include(u => u.Actor)
            .AnyAsync(u => u.Actor.CompanyId == companyId && !u.Actor.IsDeleted);
    }
}
