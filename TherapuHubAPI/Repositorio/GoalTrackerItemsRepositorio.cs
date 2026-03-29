using TherapuHubAPI.Models;
using TherapuHubAPI.Repositorio.IRepositorio;

namespace TherapuHubAPI.Repositorio;

public class GoalTrackerItemsRepositorio : Repository<GoalTrackerItems>, IGoalTrackerItemsRepositorio
{
    public GoalTrackerItemsRepositorio(ContextDB context) : base(context)
    {
    }
}
