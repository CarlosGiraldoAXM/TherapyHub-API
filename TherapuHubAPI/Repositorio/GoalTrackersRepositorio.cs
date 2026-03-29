using TherapuHubAPI.Models;
using TherapuHubAPI.Repositorio.IRepositorio;

namespace TherapuHubAPI.Repositorio;

public class GoalTrackersRepositorio : Repository<GoalTrackers>, IGoalTrackersRepositorio
{
    public GoalTrackersRepositorio(ContextDB context) : base(context)
    {
    }
}
