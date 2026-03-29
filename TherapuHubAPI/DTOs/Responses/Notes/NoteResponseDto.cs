namespace TherapuHubAPI.DTOs.Responses.Notes;

public class NoteResponseDto
{
    public long Id { get; set; }
    public int CompanyId { get; set; }
    public int CreatedByActorId { get; set; }
    public int? OwnerActorId { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = string.Empty;
    public byte PriorityId { get; set; }
    public string? PriorityName { get; set; }
    public byte NoteTypeId { get; set; }
    public int MenuId { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
    public int? SectionId { get; set; }
}
