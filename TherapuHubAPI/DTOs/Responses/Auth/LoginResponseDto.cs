using System.Text.Json.Serialization;

namespace TherapuHubAPI.DTOs.Responses.Auth;

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;

    [JsonPropertyName("tipoUsuarioId")]
    public int UserTypeId { get; set; }

    public string TipoUsuarioNombre { get; set; } = string.Empty;

    [JsonPropertyName("expiraEn")]
    public DateTime ExpiraEn { get; set; }

    /// <summary>True when user logged in with default password (123456) and must set a new password.</summary>
    [JsonPropertyName("requiresPasswordReset")]
    public bool RequiresPasswordReset { get; set; }

    /// <summary>True when user type is system (EsSistema); can create users for any company.</summary>
    [JsonPropertyName("esSistema")]
    public bool EsSistema { get; set; }
}
