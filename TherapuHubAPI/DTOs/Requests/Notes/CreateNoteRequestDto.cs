namespace TherapuHubAPI.DTOs.Requests.Notes;

public class CreateNoteRequestDto
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = string.Empty;
    public byte PriorityId { get; set; }
    public byte NoteTypeId { get; set; }
    /// <summary>ID of the NoteModule (1=Staff, 2=Clinical, etc.)</summary>
    public int ModuleId { get; set; }
    /// <summary>ID of the entity within the module (e.g. StaffId). Null or 0 means no section (module-level note).</summary>
    public int? EntityId { get; set; }
    public DateTime? DueDate { get; set; }
}
