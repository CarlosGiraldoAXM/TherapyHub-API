using System.Text.Json.Serialization;

namespace TherapuHubAPI.DTOs.Requests.Chats;

public class MarkMessagesReadRequestDto
{
    [JsonPropertyName("messageIds")]
    public List<long> MessageIds { get; set; } = new();
}
