namespace TherapuHubAPI.DTOs.Responses.Library;

public class LibraryItemResponseDto
{
    public int Id { get; set; }
    public byte CategoryId { get; set; }
    public string CategoryName { get; set; } = null!;
    public string CategorySlug { get; set; } = null!;
    public string Name { get; set; } = null!;

    // Interventions & Assessment Tools
    public string? Description { get; set; }

    // Maladaptive / Skill Acquisition / Replacement Behaviors
    public string? Barriers { get; set; }
    public string? Measurement { get; set; }

    // Maladaptive Behaviors only
    public string? Functions { get; set; }
    public string? Topography { get; set; }

    // Skill Acquisition & Replacement Behaviors
    public string? Definition { get; set; }
    public string? Objective { get; set; }
    public string? Procedures { get; set; }
    public string? TeachingMaterials { get; set; }

    public int CreatedByActorId { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }

    public List<LibraryItemFileResponseDto> Files { get; set; } = new();
}
