namespace TherapuHubAPI.DTOs.Requests.Library;

public class CreateLibraryItemRequestDto
{
    public byte CategoryId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? Barriers { get; set; }
    public string? Measurement { get; set; }
    public string? Functions { get; set; }
    public string? Topography { get; set; }
    public string? Definition { get; set; }
    public string? Objective { get; set; }
    public string? Procedures { get; set; }
    public string? TeachingMaterials { get; set; }
}
