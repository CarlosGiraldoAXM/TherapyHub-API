using Microsoft.EntityFrameworkCore;
using TherapuHubAPI.Models;
using TherapuHubAPI.Repositorio.IRepositorio;

namespace TherapuHubAPI.Repositorio;

public class ClientRepositorio : Repository<Clients>, IClientRepositorio
{
    public ClientRepositorio(ContextDB context) : base(context)
    {
    }

    public async Task<IEnumerable<Clients>> GetByCompanyIdAsync(int companyId)
    {
        return await _dbSet
            .Include(c => c.Actor)
            .Where(c => c.Actor.CompanyId == companyId && !c.Actor.IsDeleted)
            .OrderBy(c => c.Actor.FullName)
            .ToListAsync();
    }

    public async Task<Clients?> GetByIdAndCompanyAsync(int id, int companyId)
    {
        return await _dbSet
            .Include(c => c.Actor)
            .FirstOrDefaultAsync(c => c.Id == id && c.Actor.CompanyId == companyId && !c.Actor.IsDeleted);
    }

    public async Task<string> GenerateClientCodeAsync(int companyId)
    {
        var count = await _dbSet
            .Include(c => c.Actor)
            .CountAsync(c => c.Actor.CompanyId == companyId);
        return $"CLT-{companyId:D3}-{(count + 1):D4}";
    }
}

public class ClientStatusRepositorio : Repository<ClientStatuses>, IClientStatusRepositorio
{
    public ClientStatusRepositorio(ContextDB context) : base(context)
    {
    }
}
