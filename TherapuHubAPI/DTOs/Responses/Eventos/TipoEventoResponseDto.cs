using System.Text.Json.Serialization;

namespace TherapuHubAPI.DTOs.Responses.Events;

public class TipoEventoResponseDto
{
    public int IdTipoEvento { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Color { get; set; }
    public string? Icono { get; set; }

    [JsonPropertyName("activo")]
    public bool IsActive { get; set; }

    public bool IsSystem { get; set; }
}
