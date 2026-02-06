using System.Text.Json.Serialization;

namespace TherapuHubAPI.DTOs.Requests.Users;

public class UpdateUsuarioRequestDto
{
    public string Correo { get; set; } = string.Empty;
    public string? Contrasena { get; set; }
    public string? ConfirmarContrasena { get; set; }
    public string Nombre { get; set; } = string.Empty;

    [JsonPropertyName("tipoUsuarioId")]
    public int UserTypeId { get; set; }

    [JsonPropertyName("activo")]
    public bool IsActive { get; set; }
}
