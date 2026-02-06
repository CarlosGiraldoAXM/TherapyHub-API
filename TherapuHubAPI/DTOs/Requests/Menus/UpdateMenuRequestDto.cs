using System.Text.Json.Serialization;

namespace TherapuHubAPI.DTOs.Requests.Menus;

public class UpdateMenuRequestDto
{
    public string Titulo { get; set; } = string.Empty;
    /// <summary>Route for the menu. Use empty string or "#" for container-only (no navigation).</summary>
    public string Ruta { get; set; } = string.Empty;
    public string? Icono { get; set; }
    public int? Orden { get; set; }
    /// <summary>Parent menu Id for submenus. Null for top-level or container.</summary>
    public int? ParentId { get; set; }

    [JsonPropertyName("activo")]
    public bool IsActive { get; set; } = true;
}
