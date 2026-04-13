using TherapuHubAPI.Models;
using TherapuHubAPI.Repositorio.IRepositorio;

namespace TherapuHubAPI.Repositorio;

public class SessionNotesStatusRepositorio : Repository<SessionNotesStatus>, ISessionNotesStatusRepositorio
{
    public SessionNotesStatusRepositorio(ContextDB context) : base(context)
    {
    }
}
