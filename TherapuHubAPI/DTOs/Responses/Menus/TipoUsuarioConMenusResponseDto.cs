using System.Text.Json.Serialization;

namespace TherapuHubAPI.DTOs.Responses.Menus;

public class TipoUsuarioConMenusResponseDto
{
    [JsonPropertyName("tipoUsuarioId")]
    public int UserTypeId { get; set; }

    [JsonPropertyName("tipoUsuarioNombre")]
    public string TipoUsuarioNombre { get; set; } = string.Empty;

    [JsonPropertyName("menus")]
    public List<MenuResponseDto> Menus { get; set; } = new List<MenuResponseDto>();
}
