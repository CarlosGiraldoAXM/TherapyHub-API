using System.Text.Json.Serialization;

namespace TherapuHubAPI.DTOs.Requests.Companies;

public class UpdateCompaniaRequestDto
{
    public string Nombre { get; set; } = null!;
    public string? Nit { get; set; }

    [JsonPropertyName("activo")]
    public bool IsActive { get; set; }
}
