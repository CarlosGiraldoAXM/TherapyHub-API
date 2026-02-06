using System.Text.Json.Serialization;

namespace TherapuHubAPI.DTOs.Requests.Menus;

public class AsignarMenusRequestDto
{
    [JsonPropertyName("tipoUsuarioId")]
    public int UserTypeId { get; set; }

    [JsonPropertyName("menuIds")]
    public List<int> MenuIds { get; set; } = new List<int>();
}
