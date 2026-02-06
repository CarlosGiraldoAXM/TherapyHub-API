using System.Text.Json.Serialization;

namespace TherapuHubAPI.DTOs.Requests.Users;

public class CreateUsuarioRequestDto
{
    public string Correo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;

    [JsonPropertyName("tipoUsuarioId")]
    public int UserTypeId { get; set; }

    /// <summary>Optional. When the creator is system (EsSistema), they can set the company for the new user. Otherwise ignored.</summary>
    [JsonPropertyName("companiaId")]
    public int? CompanyId { get; set; }
}
