using System.Text.Json.Serialization;

namespace TherapuHubAPI.DTOs.Responses.Chats;

public class ChatMessageResponseDto
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("chatId")]
    public int ChatId { get; set; }

    [JsonPropertyName("senderUserId")]
    public int SenderUserId { get; set; }

    [JsonPropertyName("senderUserName")]
    public string SenderUserName { get; set; } = string.Empty;

    [JsonPropertyName("messageText")]
    public string MessageText { get; set; } = string.Empty;

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("isEdited")]
    public bool IsEdited { get; set; }

    [JsonPropertyName("isDeleted")]
    public bool IsDeleted { get; set; }

    [JsonPropertyName("readBy")]
    public List<MessageReadInfoDto> ReadBy { get; set; } = new();
}

public class MessageReadInfoDto
{
    [JsonPropertyName("userId")]
    public int UserId { get; set; }

    [JsonPropertyName("userName")]
    public string UserName { get; set; } = string.Empty;

    [JsonPropertyName("readAt")]
    public DateTime ReadAt { get; set; }
}
