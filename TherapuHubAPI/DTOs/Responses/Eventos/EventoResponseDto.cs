using System.Text.Json.Serialization;

namespace TherapuHubAPI.DTOs.Responses.Events;

public class EventoResponseDto
{
    public int IdEvento { get; set; }
    public string Titulo { get; set; } = null!;
    public string? Descripcion { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }

    [JsonPropertyName("esTodoElDia")]
    public bool IsAllDay { get; set; }

    public int IdTipoEvento { get; set; }
    public string? TipoEventoNombre { get; set; }
    public string? TipoEventoColor { get; set; }

    [JsonPropertyName("esGlobal")]
    public bool IsGlobal { get; set; }

    public string Estado { get; set; } = null!;

    [JsonPropertyName("fechaCreacion")]
    public DateTime CreatedAt { get; set; }
}
