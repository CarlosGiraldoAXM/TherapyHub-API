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
            .FirstOrDefaultAsync(u => u.Email.ToLower() == correo.ToLower() && !u.IsDeleted);
    }

    public new async Task<Users?> GetByIdAsync(int id)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
    }

    public new async Task<IEnumerable<Users>> GetAllAsync()
    {
        return await _dbSet
            .Where(u => !u.IsDeleted)
            .ToListAsync();
    }

    public async Task<int> CountByTipoUsuarioIdAsync(int tipoUsuarioId)
    {
        return await _dbSet
            .CountAsync(u => u.UserTypeId == tipoUsuarioId && !u.IsDeleted);
    }

    public async Task<bool> HasUsersInCompanyAsync(int companyId)
    {
        return await _dbSet
            .AnyAsync(u => u.CompanyId == companyId && !u.IsDeleted);
    }
}
