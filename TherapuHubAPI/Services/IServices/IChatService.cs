using TherapuHubAPI.DTOs.Requests.Chats;
using TherapuHubAPI.DTOs.Responses.Chats;

namespace TherapuHubAPI.Services.IServices;

public interface IChatService
{
    Task<IEnumerable<CompanyChatResponseDto>> GetChatsByCompanyAsync(int currentUserId);
    Task<CompanyChatResponseDto?> GetChatByIdAsync(int chatId, int currentUserId);
    Task<IEnumerable<ChatMessageResponseDto>> GetMessagesAsync(int chatId, int currentUserId);
    Task<ChatMessageResponseDto?> SendMessageAsync(int chatId, CreateChatMessageRequestDto request, int currentUserId);
    Task MarkMessagesAsReadAsync(MarkMessagesReadRequestDto request, int currentUserId);
    Task<int> GetUnreadMessageCountAsync(int currentUserId);
    Task<bool> DeleteMessageAsync(long messageId, int currentUserId);
}
