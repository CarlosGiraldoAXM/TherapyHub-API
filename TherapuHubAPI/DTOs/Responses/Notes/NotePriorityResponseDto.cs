namespace TherapuHubAPI.DTOs.Responses.Notes;

public class NotePriorityResponseDto
{
    public byte Id { get; set; }
    public string Name { get; set; } = null!;
    public bool IsActive { get; set; }
}
