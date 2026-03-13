using System.Text.Json.Serialization;

namespace TherapuHubAPI.DTOs.Requests.Chats;

public class EditChatMessageRequestDto
{
    [JsonPropertyName("messageText")]
    public string MessageText { get; set; } = string.Empty;
}
