using Microsoft.Extensions.Configuration;
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
    private readonly int _messageEditWindowMinutes;

    public ChatService(IUnitOfWork unitOfWork, IUsuarioRepositorio usuarioRepositorio, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _usuarioRepositorio = usuarioRepositorio;
        _messageEditWindowMinutes = configuration.GetValue<int>("ChatSettings:MessageEditWindowMinutes", 15);
    }

    public async Task<IEnumerable<CompanyChatResponseDto>> GetChatsByCompanyAsync(int currentUserId)
    {
        var user = await _usuarioRepositorio.GetByIdAsync(currentUserId);
        if (user == null) return Array.Empty<CompanyChatResponseDto>();

        var chats = (await _unitOfWork.CompanyChats.GetByCompanyIdAsync(user.Actor.CompanyId)).ToList();
        if (chats.Count == 0)
        {
            var defaultChat = new CompanyChats
            {
                CompanyId = user.Actor.CompanyId,
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
        if (user == null || chat.CompanyId != user.Actor.CompanyId) return null;

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
        if (user == null || chat.CompanyId != user.Actor.CompanyId) return Array.Empty<ChatMessageResponseDto>();

        var messages = (await _unitOfWork.ChatMessages.GetByChatIdOrderedAsync(chatId)).ToList();
        if (messages.Count == 0) return Array.Empty<ChatMessageResponseDto>();

        var messageIds = messages.Select(m => m.Id).ToList();
        var reads = (await _unitOfWork.MessageReads.GetByMessageIdsAsync(messageIds)).ToList();

        var senderIds = messages.Select(m => m.SenderUserId).Distinct().ToList();
        var editorIds = messages.Where(m => m.EditedUserId.HasValue).Select(m => m.EditedUserId!.Value).Distinct().ToList();
        var readers = reads.Select(r => r.UserId).Distinct().ToList();
        var userIds = senderIds.Union(editorIds).Union(readers).Distinct().ToList();
        var users = (await _usuarioRepositorio.FindAsync(u => userIds.Contains(u.Id))).ToDictionary(u => u.Id);

        var editWindowCutoff = DateTime.UtcNow.AddMinutes(-_messageEditWindowMinutes);

        return messages.Select(m =>
        {
            var dto = new ChatMessageResponseDto
            {
                Id = m.Id,
                ChatId = m.ChatId,
                SenderUserId = m.SenderUserId,
                SenderUserName = users.TryGetValue(m.SenderUserId, out var u) ? u.Actor.FullName : "",
                MessageText = m.MessageText,
                CreatedAt = m.CreatedAt,
                IsEdited = m.IsEdited,
                EditedAt = m.EditedAt,
                EditedUserId = m.EditedUserId,
                EditedUserName = m.EditedUserId.HasValue && users.TryGetValue(m.EditedUserId.Value, out var eu) ? eu.Actor.FullName : null,
                CanEdit = m.SenderUserId == currentUserId && !m.IsDeleted && m.CreatedAt >= editWindowCutoff,
                IsDeleted = m.IsDeleted
            };
            dto.ReadBy = reads.Where(r => r.MessageId == m.Id && r.UserId != m.SenderUserId).Select(r => new MessageReadInfoDto
            {
                UserId = r.UserId,
                UserName = users.TryGetValue(r.UserId, out var ru) ? ru.Actor.FullName : "",
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
        if (user == null || chat.CompanyId != user.Actor.CompanyId) return null;

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
            SenderUserName = user.Actor.FullName,
            MessageText = message.MessageText,
            CreatedAt = message.CreatedAt,
            IsEdited = message.IsEdited,
            CanEdit = true,
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
            if (chat == null || chat.CompanyId != user.Actor.CompanyId) continue;

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

        var chats = await _unitOfWork.CompanyChats.GetByCompanyIdAsync(user.Actor.CompanyId);
        var chatIds = chats.Select(c => c.Id).ToList();
        return await _unitOfWork.ChatMessages.GetUnreadCountForUserAsync(chatIds, currentUserId);
    }

    public async Task<bool> DeleteMessageAsync(long messageId, int currentUserId)
    {
        var message = await _unitOfWork.ChatMessages.GetByIdAsync(messageId);
        if (message == null) return false;

        // Only the sender can delete their own message
        if (message.SenderUserId != currentUserId) return false;

        message.IsDeleted = true;
        message.DeleteUserId = currentUserId;
        message.DeletedAt = DateTime.UtcNow;

        _unitOfWork.ChatMessages.Update(message);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<(ChatMessageResponseDto? Message, string? Error)> EditMessageAsync(long messageId, EditChatMessageRequestDto request, int currentUserId)
    {
        var message = await _unitOfWork.ChatMessages.GetByIdAsync(messageId);
        if (message == null)
            return (null, "Mensaje no encontrado.");

        if (message.SenderUserId != currentUserId)
            return (null, "Solo el autor puede editar este mensaje.");

        if (message.IsDeleted)
            return (null, "No se puede editar un mensaje eliminado.");

        var editDeadline = message.CreatedAt.AddMinutes(_messageEditWindowMinutes);
        if (DateTime.UtcNow > editDeadline)
            return (null, $"El mensaje solo puede editarse dentro de los primeros {_messageEditWindowMinutes} minutos tras su envío.");

        message.MessageText = request.MessageText.Trim();
        message.IsEdited = true;
        message.EditedUserId = currentUserId;
        message.EditedAt = DateTime.UtcNow;

        _unitOfWork.ChatMessages.Update(message);
        await _unitOfWork.SaveChangesAsync();

        var user = await _usuarioRepositorio.GetByIdAsync(currentUserId);

        return (new ChatMessageResponseDto
        {
            Id = message.Id,
            ChatId = message.ChatId,
            SenderUserId = message.SenderUserId,
            SenderUserName = user?.Actor.FullName ?? "",
            MessageText = message.MessageText,
            CreatedAt = message.CreatedAt,
            IsEdited = message.IsEdited,
            EditedAt = message.EditedAt,
            EditedUserId = message.EditedUserId,
            EditedUserName = user?.Actor.FullName,
            CanEdit = message.CreatedAt.AddMinutes(_messageEditWindowMinutes) > DateTime.UtcNow,
            IsDeleted = message.IsDeleted,
            ReadBy = new List<MessageReadInfoDto>()
        }, null);
    }
}
