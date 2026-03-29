using TherapuHubAPI.Models;
using TherapuHubAPI.Repositorio.IRepositorio;

namespace TherapuHubAPI.Repositorio;

public class GoalTrackerCategoriesRepositorio : Repository<GoalTrackerCategories>, IGoalTrackerCategoriesRepositorio
{
    public GoalTrackerCategoriesRepositorio(ContextDB context) : base(context)
    {
    }
}
