using System.Text.Json.Serialization;

namespace TherapuHubAPI.DTOs.Requests.Events;

public class CreateEventoRequestDto
{
    public string Titulo { get; set; } = null!;
    public string? Descripcion { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }

    [JsonPropertyName("esTodoElDia")]
    public bool IsAllDay { get; set; }

    public int IdTipoEvento { get; set; }

    [JsonPropertyName("esGlobal")]
    public bool IsGlobal { get; set; }

    public string? OtherType { get; set; }

    public bool? IsPrivate { get; set; }

    [JsonPropertyName("usuariosIds")]
    public List<int>? UsuariosIds { get; set; }
}
