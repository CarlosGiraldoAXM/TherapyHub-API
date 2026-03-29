namespace TherapuHubAPI.DTOs.Requests.SessionNotes;

public class CreateSessionNoteRequestDto
{
    public int RbtActorId { get; set; }
    public int ClientActorId { get; set; }
    public DateTime SessionDate { get; set; }
    public int StatusId { get; set; }
    public string? Notes { get; set; }
    public string? Actions { get; set; }
}
