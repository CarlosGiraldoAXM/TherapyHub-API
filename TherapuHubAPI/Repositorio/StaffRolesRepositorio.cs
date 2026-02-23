using TherapuHubAPI.Models;
using TherapuHubAPI.Repositorio.IRepositorio;

namespace TherapuHubAPI.Repositorio;

public class StaffRolesRepositorio : Repository<StaffRoles>, IStaffRolesRepositorio
{
    public StaffRolesRepositorio(ContextDB context) : base(context)
    {
    }
}
