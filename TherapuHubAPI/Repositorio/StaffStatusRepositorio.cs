using Microsoft.EntityFrameworkCore;
using TherapuHubAPI.Models;
using TherapuHubAPI.Repositorio.IRepositorio;

namespace TherapuHubAPI.Repositorio;

public class StaffStatusRepositorio : Repository<StaffStatus>, IStaffStatusRepositorio
{
    public StaffStatusRepositorio(ContextDB context) : base(context)
    {
    }
}
