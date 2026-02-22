using TherapuHubAPI.Models;

namespace TherapuHubAPI.Repositorio.IRepositorio;

public interface ICompanyChatsRepositorio : IRepository<CompanyChats>
{
    Task<IEnumerable<CompanyChats>> GetByCompanyIdAsync(int companyId);
}
