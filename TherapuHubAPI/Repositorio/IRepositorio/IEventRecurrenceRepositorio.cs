using TherapuHubAPI.Models;

namespace TherapuHubAPI.Repositorio.IRepositorio;

public interface IEventRecurrenceRepositorio : IRepository<EventRecurrence>
{
    Task<IEnumerable<EventRecurrence>> GetByEventIdAsync(int eventId);
}
