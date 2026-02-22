using TherapuHubAPI.DTOs.Requests.Chats;
using TherapuHubAPI.DTOs.Responses.Chats;
using TherapuHubAPI.Models;
using TherapuHubAPI.Repositorio;
using TherapuHubAPI.Repositorio.IRepositorio;
using TherapuHubAPI.Services.IServices;

namespace TherapuHubAPI.Services;

public class ChatService : IChatService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUsuarioRepositorio _usuarioRepositorio;

    public ChatService(IUnitOfWork unitOfWork, IUsuarioRepositorio usuarioRepositorio)
    {
        _unitOfWork = unitOfWork;
        _usuarioRepositorio = usuarioRepositorio;
    }

    public async Task<IEnumerable<CompanyChatResponseDto>> GetChatsByCompanyAsync(int currentUserId)
    {
        var user = await _usuarioRepositorio.GetByIdAsync(currentUserId);
        if (user == null) return Array.Empty<CompanyChatResponseDto>();

        var chats = (await _unitOfWork.CompanyChats.GetByCompanyIdAsync(user.CompanyId)).ToList();
        if (chats.Count == 0)
        {
            var defaultChat = new CompanyChats
            {
                CompanyId = user.CompanyId,
                Name = "General",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
            await _unitOfWork.CompanyChats.AddAsync(defaultChat);
            await _unitOfWork.SaveChangesAsync();
            chats.Add(defaultChat);
        }

        return chats.Select(c => new CompanyChatResponseDto
        {
            Id = c.Id,
            CompanyId = c.CompanyId,
            Name = c.Name,
            CreatedAt = c.CreatedAt,
            IsActive = c.IsActive
        }).ToList();
    }

    public async Task<CompanyChatResponseDto?> GetChatByIdAsync(int chatId, int currentUserId)
    {
        var chat = await _unitOfWork.CompanyChats.GetByIdAsync(chatId);
        if (chat == null) return null;

        var user = await _usuarioRepositorio.GetByIdAsync(currentUserId);
        if (user == null || chat.CompanyId != user.CompanyId) return null;

        return new CompanyChatResponseDto
        {
            Id = chat.Id,
            CompanyId = chat.CompanyId,
            Name = chat.Name,
            CreatedAt = chat.CreatedAt,
            IsActive = chat.IsActive
        };
    }

    public async Task<IEnumerable<ChatMessageResponseDto>> GetMessagesAsync(int chatId, int currentUserId)
    {
        var chat = await _unitOfWork.CompanyChats.GetByIdAsync(chatId);
        if (chat == null) return Array.Empty<ChatMessageResponseDto>();

        var user = await _usuarioRepositorio.GetByIdAsync(currentUserId);
        if (user == null || chat.CompanyId != user.CompanyId) return Array.Empty<ChatMessageResponseDto>();

        var messages = (await _unitOfWork.ChatMessages.GetByChatIdOrderedAsync(chatId)).ToList();
        if (messages.Count == 0) return Array.Empty<ChatMessageResponseDto>();

        var messageIds = messages.Select(m => m.Id).ToList();
        var reads = (await _unitOfWork.MessageReads.GetByMessageIdsAsync(messageIds)).ToList();

        var senderIds = messages.Select(m => m.SenderUserId).Distinct().ToList();
        var readers = reads.Select(r => r.UserId).Distinct().ToList();
        var userIds = senderIds.Union(readers).Distinct().ToList();
        var users = (await _usuarioRepositorio.FindAsync(u => userIds.Contains(u.Id))).ToDictionary(u => u.Id);

        return messages.Select(m =>
        {
            var dto = new ChatMessageResponseDto
            {
                Id = m.Id,
                ChatId = m.ChatId,
                SenderUserId = m.SenderUserId,
                SenderUserName = users.TryGetValue(m.SenderUserId, out var u) ? u.FullName : "",
                MessageText = m.MessageText,
                CreatedAt = m.CreatedAt,
                IsEdited = m.IsEdited,
                IsDeleted = m.IsDeleted
            };
            dto.ReadBy = reads.Where(r => r.MessageId == m.Id).Select(r => new MessageReadInfoDto
            {
                UserId = r.UserId,
                UserName = users.TryGetValue(r.UserId, out var ru) ? ru.FullName : "",
                ReadAt = r.ReadAt
            }).OrderBy(x => x.ReadAt).ToList();
            return dto;
        }).ToList();
    }

    public async Task<ChatMessageResponseDto?> SendMessageAsync(int chatId, CreateChatMessageRequestDto request, int currentUserId)
    {
        var chat = await _unitOfWork.CompanyChats.GetByIdAsync(chatId);
        if (chat == null) return null;

        var user = await _usuarioRepositorio.GetByIdAsync(currentUserId);
        if (user == null || chat.CompanyId != user.CompanyId) return null;

        var message = new ChatMessages
        {
            ChatId = chatId,
            SenderUserId = currentUserId,
            MessageText = request.MessageText?.Trim() ?? "",
            CreatedAt = DateTime.UtcNow,
            IsEdited = false,
            IsDeleted = false
        };

        await _unitOfWork.ChatMessages.AddAsync(message);
        await _unitOfWork.SaveChangesAsync();

        return new ChatMessageResponseDto
        {
            Id = message.Id,
            ChatId = message.ChatId,
            SenderUserId = message.SenderUserId,
            SenderUserName = user.FullName,
            MessageText = message.MessageText,
            CreatedAt = message.CreatedAt,
            IsEdited = message.IsEdited,
            IsDeleted = message.IsDeleted,
            ReadBy = new List<MessageReadInfoDto>()
        };
    }

    public async Task MarkMessagesAsReadAsync(MarkMessagesReadRequestDto request, int currentUserId)
    {
        if (request.MessageIds == null || request.MessageIds.Count == 0) return;

        var user = await _usuarioRepositorio.GetByIdAsync(currentUserId);
        if (user == null) return;

        var existingReads = (await _unitOfWork.MessageReads.FindAsync(r =>
            r.UserId == currentUserId && request.MessageIds.Contains(r.MessageId))).ToList();
        var alreadyReadIds = existingReads.Select(r => r.MessageId).ToHashSet();

        foreach (var messageId in request.MessageIds.Distinct())
        {
            if (alreadyReadIds.Contains(messageId)) continue;

            var message = await _unitOfWork.ChatMessages.GetByIdAsync(messageId);
            if (message == null) continue;

            var chat = await _unitOfWork.CompanyChats.GetByIdAsync(message.ChatId);
            if (chat == null || chat.CompanyId != user.CompanyId) continue;

            await _unitOfWork.MessageReads.AddAsync(new MessageReads
            {
                MessageId = messageId,
                UserId = currentUserId,
                ReadAt = DateTime.UtcNow
            });
            alreadyReadIds.Add(messageId);
        }

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<int> GetUnreadMessageCountAsync(int currentUserId)
    {
        var user = await _usuarioRepositorio.GetByIdAsync(currentUserId);
        if (user == null) return 0;

        var chats = await _unitOfWork.CompanyChats.GetByCompanyIdAsync(user.CompanyId);
        var chatIds = chats.Select(c => c.Id).ToList();
        return await _unitOfWork.ChatMessages.GetUnreadCountForUserAsync(chatIds, currentUserId);
    }
}
