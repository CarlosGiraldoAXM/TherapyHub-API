using System.Text.Json.Serialization;

namespace TherapuHubAPI.DTOs.Responses.Menus;

public class MenuResponseDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Ruta { get; set; } = string.Empty;
    public string? Icono { get; set; }
    public int? Orden { get; set; }
    public int? ParentId { get; set; }

    [JsonPropertyName("activo")]
    public bool IsActive { get; set; }

    public bool IsSystem { get; set; }

    [JsonPropertyName("fechaCreacion")]
    public DateTime CreatedAt { get; set; }

    /// <summary>Only populated when returning tree for sidebar (usuario actual / tipo usuario).</summary>
    public List<MenuResponseDto> Children { get; set; } = new List<MenuResponseDto>();
}
