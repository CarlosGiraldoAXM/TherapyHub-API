using TherapuHubAPI.Models;

namespace TherapuHubAPI.Repositorio.IRepositorio;

public interface IMessageReadsRepositorio : IRepository<MessageReads>
{
    Task<IEnumerable<MessageReads>> GetByMessageIdsAsync(IEnumerable<long> messageIds);
}
