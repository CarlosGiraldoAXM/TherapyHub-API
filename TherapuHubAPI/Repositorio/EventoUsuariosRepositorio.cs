using TherapuHubAPI.Models;
using TherapuHubAPI.Repositorio.IRepositorio;

namespace TherapuHubAPI.Repositorio;

public class EventoUsuariosRepositorio : Repository<EventUsers>, IEventoUsuariosRepositorio
{
    public EventoUsuariosRepositorio(ContextDB context) : base(context)
    {
    }
}
