using Microsoft.EntityFrameworkCore;
using TherapuHubAPI.Models;
using TherapuHubAPI.Repositorio.IRepositorio;

namespace TherapuHubAPI.Repositorio;

public class ChatMessagesRepositorio : Repository<ChatMessages>, IChatMessagesRepositorio
{
    private readonly ContextDB _contextDb;

    public ChatMessagesRepositorio(ContextDB context) : base(context)
    {
        _contextDb = context;
    }

    public async Task<ChatMessages?> GetByIdAsync(long id)
    {
        return await _contextDb.ChatMessages.FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<IEnumerable<ChatMessages>> GetByChatIdOrderedAsync(int chatId)
    {
        return await _contextDb.ChatMessages
            .Where(m => m.ChatId == chatId)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync();
    }

    public async Task<int> GetUnreadCountForUserAsync(IEnumerable<int> chatIds, int userId)
    {
        var chatIdsList = chatIds.ToList();
        if (chatIdsList.Count == 0) return 0;
        return await _contextDb.ChatMessages
            .Where(m =>
                chatIdsList.Contains(m.ChatId) && !m.IsDeleted && m.SenderUserId != userId
                && !_contextDb.MessageReads.Any(r => r.MessageId == m.Id && r.UserId == userId))
            .CountAsync();
    }
}
