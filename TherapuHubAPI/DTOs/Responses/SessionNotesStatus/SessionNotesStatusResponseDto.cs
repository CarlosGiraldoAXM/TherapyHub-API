namespace TherapuHubAPI.DTOs.Responses.SessionNotesStatus;

public class SessionNotesStatusResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Color { get; set; } = null!;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
