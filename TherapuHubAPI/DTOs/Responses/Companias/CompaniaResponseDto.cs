using System.Text.Json.Serialization;

namespace TherapuHubAPI.DTOs.Responses.Companies;

public class CompaniaResponseDto
{
    [JsonPropertyName("idCompania")]
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;
    public string? Nit { get; set; }
    public int? UserLimit { get; set; }

    [JsonPropertyName("activo")]
    public bool IsActive { get; set; }

    [JsonPropertyName("fechaCreacion")]
    public DateTime CreatedAt { get; set; }
}
