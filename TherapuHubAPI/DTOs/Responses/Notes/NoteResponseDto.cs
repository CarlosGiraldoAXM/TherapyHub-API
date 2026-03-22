namespace TherapuHubAPI.DTOs.Responses.Notes;

public class NoteResponseDto
{
    public long Id { get; set; }
    public int CompanyId { get; set; }
    public int CreatedByUserId { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = string.Empty;
    public byte PriorityId { get; set; }
    public string? PriorityName { get; set; }
    public byte NoteTypeId { get; set; }
    public int ModuleId { get; set; }
    public int? SectionId { get; set; }
    /// <summary>EntityId derived from the section Name (parsed int). Equals the original entity (e.g. StaffId) used at creation.</summary>
    public int EntityId { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}
