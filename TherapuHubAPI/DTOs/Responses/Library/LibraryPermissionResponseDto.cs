namespace TherapuHubAPI.DTOs.Responses.Library;

public class LibraryPermissionResponseDto
{
    public int Id { get; set; }
    public int ActorId { get; set; }
    public string ActorName { get; set; } = string.Empty;
    public string? ActorEmail { get; set; }
    public bool CanCreate { get; set; }
    public bool CanEdit { get; set; }
    public bool CanDelete { get; set; }
    public int AssignedBy { get; set; }
    public string AssignedByName { get; set; } = string.Empty;
    public DateTime AssignedAt { get; set; }
}
