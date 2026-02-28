using Microsoft.EntityFrameworkCore;
using TherapuHubAPI.Models;
using TherapuHubAPI.Repositorio.IRepositorio;

namespace TherapuHubAPI.Repositorio;

public class EventRecurrenceRepositorio : Repository<EventRecurrence>, IEventRecurrenceRepositorio
{
    private readonly ContextDB _contextDb;

    public EventRecurrenceRepositorio(ContextDB context) : base(context)
    {
        _contextDb = context;
    }

    public async Task<IEnumerable<EventRecurrence>> GetByEventIdAsync(int eventId)
    {
        return await _contextDb.EventRecurrence
            .Where(r => r.EventId == eventId)
            .ToListAsync();
    }
}
