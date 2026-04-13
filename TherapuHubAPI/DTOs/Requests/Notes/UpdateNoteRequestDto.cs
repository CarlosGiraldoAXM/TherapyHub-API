namespace TherapuHubAPI.DTOs.Requests.Notes;

public class UpdateNoteRequestDto
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = string.Empty;
    public byte PriorityId { get; set; }
    public DateTime? DueDate { get; set; }
    public bool IsActive { get; set; }
    public bool IsPrivate { get; set; }
    public int? CategoryId { get; set; }
}
