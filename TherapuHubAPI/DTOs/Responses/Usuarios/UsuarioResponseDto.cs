using System.Text.Json.Serialization;

namespace TherapuHubAPI.DTOs.Responses.Users;

public class UsuarioResponseDto
{
    public int Id { get; set; }
    public string Correo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;

    [JsonPropertyName("tipoUsuarioId")]
    public int UserTypeId { get; set; }

    public string TipoUsuarioNombre { get; set; } = string.Empty;

    [JsonPropertyName("fechaCreacion")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("activo")]
    public bool IsActive { get; set; }

    [JsonPropertyName("companiaId")]
    public int CompanyId { get; set; }
}
