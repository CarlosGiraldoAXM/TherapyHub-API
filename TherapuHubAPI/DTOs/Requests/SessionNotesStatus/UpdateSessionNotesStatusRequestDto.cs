namespace TherapuHubAPI.DTOs.Requests.SessionNotesStatus;

public class UpdateSessionNotesStatusRequestDto
{
    public string Name { get; set; } = null!;
    public string Color { get; set; } = null!;
    public bool IsActive { get; set; }
}
