using System.Text.Json.Serialization;

namespace TherapuHubAPI.DTOs.Requests.Chats;

public class CreateChatMessageRequestDto
{
    [JsonPropertyName("messageText")]
    public string MessageText { get; set; } = string.Empty;
}
