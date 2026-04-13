namespace TherapuHubAPI.DTOs.Requests.SessionNotesStatus;

public class CreateSessionNotesStatusRequestDto
{
    public string Name { get; set; } = null!;
    public string Color { get; set; } = "#6366f1";
    public bool IsActive { get; set; } = true;
}
