using TherapuHubAPI.Models;

namespace TherapuHubAPI.Repositorio.IRepositorio;

public interface IStaffRepositorio : IRepository<Staff>
{
    Task<IEnumerable<Staff>> GetByCompanyIdAsync(int companyId);
    Task<Staff?> GetByIdAndCompanyAsync(int id, int companyId);
}
