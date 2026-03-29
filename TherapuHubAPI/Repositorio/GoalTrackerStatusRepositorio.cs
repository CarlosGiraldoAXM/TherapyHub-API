using TherapuHubAPI.Models;
using TherapuHubAPI.Repositorio.IRepositorio;

namespace TherapuHubAPI.Repositorio;

public class GoalTrackerStatusRepositorio : Repository<GoalTrackerStatus>, IGoalTrackerStatusRepositorio
{
    public GoalTrackerStatusRepositorio(ContextDB context) : base(context)
    {
    }
}
