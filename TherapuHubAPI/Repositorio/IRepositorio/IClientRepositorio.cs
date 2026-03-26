using TherapuHubAPI.Models;

namespace TherapuHubAPI.Repositorio.IRepositorio;

public interface IClientRepositorio : IRepository<Clients>
{
    Task<IEnumerable<Clients>> GetByCompanyIdAsync(int companyId);
    Task<Clients?> GetByIdAndCompanyAsync(int id, int companyId);
    Task<string> GenerateClientCodeAsync(int companyId);
}

public interface IClientStatusRepositorio : IRepository<ClientStatuses>
{
}
