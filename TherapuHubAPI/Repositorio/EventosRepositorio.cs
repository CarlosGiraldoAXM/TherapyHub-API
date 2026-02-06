using Microsoft.EntityFrameworkCore;
using TherapuHubAPI.Models;
using TherapuHubAPI.Repositorio.IRepositorio;

namespace TherapuHubAPI.Repositorio;

public class EventosRepositorio : Repository<Events>, IEventosRepositorio
{
    private readonly ContextDB _contextDb;

    public EventosRepositorio(ContextDB context) : base(context)
    {
        _contextDb = context;
    }

    public async Task<IEnumerable<Events>> GetEventosByUserAsync(int userId, int userCompaniaId, DateTime? start, DateTime? end, bool? esTodoElDia = null)
    {
        var query = _contextDb.Events.AsQueryable();

        // Global events: only visible to users of the same company as the creator. Non-global: only assigned users.
        query = query.Where(e =>
            (e.IsGlobal && e.CompanyId == userCompaniaId) ||
            (!e.IsGlobal && _contextDb.EventUsers.Any(eu => eu.EventId == e.Id && eu.UserId == userId)));

        // Filtro por esTodoElDia
        // null = todos los eventos
        // true = solo eventos de todo el día
        // false = solo eventos con hora específica
        if (esTodoElDia.HasValue)
        {
            query = query.Where(e => e.IsAllDay == esTodoElDia.Value);
        }

        if (start.HasValue)
        {
            query = query.Where(e => e.StartDate >= start.Value);
        }

        if (end.HasValue)
        {
            query = query.Where(e => e.EndDate <= end.Value);
        }

        return await query.ToListAsync();
    }
}
