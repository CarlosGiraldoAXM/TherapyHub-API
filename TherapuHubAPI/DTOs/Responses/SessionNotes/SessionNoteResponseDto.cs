namespace TherapuHubAPI.DTOs.Responses.SessionNotes;

public class SessionNoteResponseDto
{
    public long Id { get; set; }
    public int RbtActorId { get; set; }
    public int ClientActorId { get; set; }
    public DateTime SessionDate { get; set; }
    public int StatusId { get; set; }
    public string? StatusName { get; set; }
    public string? StatusColor { get; set; }
    public string? Notes { get; set; }
    public string? Actions { get; set; }
    public int CreatedByActorId { get; set; }
    public DateTime CreatedAt { get; set; }
}
