using System.Text.Json.Serialization;

namespace TherapuHubAPI.DTOs.Requests.Companies;

public class CreateCompaniaRequestDto
{
    public string Nombre { get; set; } = null!;
    public string? Nit { get; set; }
    public int? UserLimit { get; set; }

    [JsonPropertyName("activo")]
    public bool IsActive { get; set; } = true;
}
