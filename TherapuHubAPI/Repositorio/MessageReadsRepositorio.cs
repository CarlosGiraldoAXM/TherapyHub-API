using Microsoft.EntityFrameworkCore;
using TherapuHubAPI.Models;
using TherapuHubAPI.Repositorio.IRepositorio;

namespace TherapuHubAPI.Repositorio;

public class MessageReadsRepositorio : Repository<MessageReads>, IMessageReadsRepositorio
{
    private readonly ContextDB _contextDb;

    public MessageReadsRepositorio(ContextDB context) : base(context)
    {
        _contextDb = context;
    }

    public async Task<IEnumerable<MessageReads>> GetByMessageIdsAsync(IEnumerable<long> messageIds)
    {
        var ids = messageIds.Distinct().ToList();
        if (ids.Count == 0) return Array.Empty<MessageReads>();
        return await _contextDb.MessageReads
            .Where(r => ids.Contains(r.MessageId))
            .ToListAsync();
    }
}
