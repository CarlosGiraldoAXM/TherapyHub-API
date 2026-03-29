namespace TherapuHubAPI.DTOs.Requests.SessionNotes;

public class UpdateSessionNoteRequestDto
{
    public int ClientActorId { get; set; }
    public int StatusId { get; set; }
    public string? Notes { get; set; }
    public string? Actions { get; set; }
}
