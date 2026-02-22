using Microsoft.EntityFrameworkCore;
using TherapuHubAPI.Models;
using TherapuHubAPI.Repositorio.IRepositorio;

namespace TherapuHubAPI.Repositorio;

public class CompanyChatsRepositorio : Repository<CompanyChats>, ICompanyChatsRepositorio
{
    private readonly ContextDB _contextDb;

    public CompanyChatsRepositorio(ContextDB context) : base(context)
    {
        _contextDb = context;
    }

    public async Task<IEnumerable<CompanyChats>> GetByCompanyIdAsync(int companyId)
    {
        return await _contextDb.CompanyChats
            .Where(c => c.CompanyId == companyId && c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }
}
