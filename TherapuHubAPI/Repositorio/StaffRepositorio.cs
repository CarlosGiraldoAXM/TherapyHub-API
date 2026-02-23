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
            .Where(s => s.CompanyId == companyId)
            .OrderBy(s => s.LastName)
            .ThenBy(s => s.FirstName)
            .ToListAsync();
    }

    public async Task<Staff?> GetByIdAndCompanyAsync(int id, int companyId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(s => s.Id == id && s.CompanyId == companyId);
    }
}
