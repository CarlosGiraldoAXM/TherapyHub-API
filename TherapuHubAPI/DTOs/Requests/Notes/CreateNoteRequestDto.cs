namespace TherapuHubAPI.DTOs.Requests.Notes;

public class CreateNoteRequestDto
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = string.Empty;
    public byte PriorityId { get; set; }
    public byte NoteTypeId { get; set; }
    /// <summary>ID of the Menu this note belongs to.</summary>
    public int MenuId { get; set; }
    /// <summary>ID of the entity within the module (e.g. StaffId). Null or 0 means no section (module-level note).</summary>
    public int? EntityId { get; set; }
    public DateTime? DueDate { get; set; }
    /// <summary>ActorId of the entity that owns this note (Staff/Client). Null or 0 means owner = creator.</summary>
    public int? OwnerActorId { get; set; }
    /// <summary>Optional section identifier (e.g. 1 = PM tab). Null means no section.</summary>
    public int? SectionId { get; set; }
    /// <summary>When true, only the creator can see this note.</summary>
    public bool IsPrivate { get; set; } = false;
    /// <summary>Optional category. Only used in sections that allow categories.</summary>
    public int? CategoryId { get; set; }
}
