using Microsoft.EntityFrameworkCore;
using TherapuHubAPI.Models;
using TherapuHubAPI.Repositorio.IRepositorio;

namespace TherapuHubAPI.Repositorio;

public class StaffRepositorio : Repository<Staff>, IStaffRepositorio
{
    public StaffRepositorio(ContextDB context) : base(context)
    {
    }

    public async Task<IEnumerable<Staff>> GetByCompanyIdAsync(int companyId)
    {
        return await _dbSet
            .Include(s => s.Actor)
            .Where(s => s.Actor.CompanyId == companyId && !s.Actor.IsDeleted)
            .OrderBy(s => s.Actor.FullName)
            .ToListAsync();
    }

    public async Task<Staff?> GetByIdAndCompanyAsync(int id, int companyId)
    {
        return await _dbSet
            .Include(s => s.Actor)
            .FirstOrDefaultAsync(s => s.Id == id && s.Actor.CompanyId == companyId && !s.Actor.IsDeleted);
    }
}
