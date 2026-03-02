using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TherapuHubAPI.DTOs.Common;
using TherapuHubAPI.DTOs.Requests.Chats;
using TherapuHubAPI.DTOs.Responses.Chats;
using TherapuHubAPI.Services.IServices;

namespace TherapuHubAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ChatsController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatsController(IChatService chatService)
    {
        _chatService = chatService;
    }

    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim?.Value) || !int.TryParse(userIdClaim.Value, out int userId))
            return null;
        return userId;
    }

    /// <summary>Unread message count for the current user (messages from others in company chats not yet read).</summary>
    [HttpGet("unread-count")]
    [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<int>>> GetUnreadCount()
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId == null) return Unauthorized();

        var count = await _chatService.GetUnreadMessageCountAsync(currentUserId.Value);
        return Ok(new ApiResponse<int>
        {
            Success = true,
            Data = count,
            Message = "Unread count retrieved"
        });
    }

    /// <summary>Lista los chats de la compañía del usuario actual.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<CompanyChatResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<CompanyChatResponseDto>>>> GetChats()
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId == null) return Unauthorized();

        var chats = await _chatService.GetChatsByCompanyAsync(currentUserId.Value);
        return Ok(new ApiResponse<IEnumerable<CompanyChatResponseDto>>
        {
            Success = true,
            Data = chats,
            Message = "Chats obtenidos correctamente"
        });
    }

    /// <summary>Obtiene un chat por id (solo si pertenece a la compañía del usuario).</summary>
    [HttpGet("{chatId:int}")]
    [ProducesResponseType(typeof(ApiResponse<CompanyChatResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<CompanyChatResponseDto>>> GetChat(int chatId)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId == null) return Unauthorized();

        var chat = await _chatService.GetChatByIdAsync(chatId, currentUserId.Value);
        if (chat == null)
            return NotFound(ApiResponse<CompanyChatResponseDto>.NotFoundResponse("Chat no encontrado o sin acceso."));

        return Ok(new ApiResponse<CompanyChatResponseDto> { Success = true, Data = chat });
    }

    /// <summary>Obtiene los mensajes de un chat con información de lectura.</summary>
    [HttpGet("{chatId:int}/messages")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ChatMessageResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<ChatMessageResponseDto>>>> GetMessages(int chatId)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId == null) return Unauthorized();

        var messages = await _chatService.GetMessagesAsync(chatId, currentUserId.Value);
        return Ok(new ApiResponse<IEnumerable<ChatMessageResponseDto>>
        {
            Success = true,
            Data = messages,
            Message = "Mensajes obtenidos correctamente"
        });
    }

    /// <summary>Envía un mensaje en un chat.</summary>
    [HttpPost("{chatId:int}/messages")]
    [ProducesResponseType(typeof(ApiResponse<ChatMessageResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ChatMessageResponseDto>>> SendMessage(int chatId, [FromBody] CreateChatMessageRequestDto request)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId == null) return Unauthorized();

        if (string.IsNullOrWhiteSpace(request?.MessageText))
            return BadRequest(ApiResponse<ChatMessageResponseDto>.ErrorResponse("El texto del mensaje es requerido.", null, 400));

        var message = await _chatService.SendMessageAsync(chatId, request, currentUserId.Value);
        if (message == null)
            return NotFound(ApiResponse<ChatMessageResponseDto>.NotFoundResponse("Chat no encontrado o sin acceso."));

        return CreatedAtAction(nameof(GetMessages), new { chatId }, new ApiResponse<ChatMessageResponseDto>
        {
            Success = true,
            Data = message,
            Message = "Mensaje enviado"
        });
    }

    /// <summary>Soft-delete de un mensaje. Solo puede hacerlo el remitente.</summary>
    [HttpDelete("messages/{messageId:long}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<object>>> DeleteMessage(long messageId)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId == null) return Unauthorized();

        var deleted = await _chatService.DeleteMessageAsync(messageId, currentUserId.Value);
        if (!deleted)
            return BadRequest(ApiResponse<object>.ErrorResponse("Cannot delete this message. It may not exist or you are not the sender.", null, 400));

        return Ok(new ApiResponse<object> { Success = true, Message = "Message deleted" });
    }

    /// <summary>Marca mensajes como leídos por el usuario actual.</summary>
    [HttpPost("messages/read")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<object>>> MarkMessagesAsRead([FromBody] MarkMessagesReadRequestDto request)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId == null) return Unauthorized();

        await _chatService.MarkMessagesAsReadAsync(request ?? new MarkMessagesReadRequestDto(), currentUserId.Value);
        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Mensajes marcados como leídos"
        });
    }
}
