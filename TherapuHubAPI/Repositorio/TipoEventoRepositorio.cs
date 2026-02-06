using TherapuHubAPI.Models;
using TherapuHubAPI.Repositorio.IRepositorio;

namespace TherapuHubAPI.Repositorio;

public class TipoEventoRepositorio : Repository<EventTypes>, ITipoEventoRepositorio
{
    public TipoEventoRepositorio(ContextDB context) : base(context)
    {
    }
}
