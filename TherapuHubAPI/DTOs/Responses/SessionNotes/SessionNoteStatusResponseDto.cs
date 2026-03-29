namespace TherapuHubAPI.DTOs.Responses.SessionNotes;

public class SessionNoteStatusResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Color { get; set; } = null!;
    public bool IsActive { get; set; }
}
