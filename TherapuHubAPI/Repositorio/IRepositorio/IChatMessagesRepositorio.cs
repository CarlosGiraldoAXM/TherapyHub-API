using TherapuHubAPI.Models;

namespace TherapuHubAPI.Repositorio.IRepositorio;

public interface IChatMessagesRepositorio : IRepository<ChatMessages>
{
    Task<ChatMessages?> GetByIdAsync(long id);
    Task<IEnumerable<ChatMessages>> GetByChatIdOrderedAsync(int chatId);
    Task<int> GetUnreadCountForUserAsync(IEnumerable<int> chatIds, int userId);
}
